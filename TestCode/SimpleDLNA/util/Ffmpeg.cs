﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
//using System.Drawing;
using System.Windows;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
//using log4net;

namespace NMaier.SimpleDlna.Utilities
{
  [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Fmpeg")]
  public static class FFmpeg
  {
    private static readonly DirectoryInfo[] specialLocations = new DirectoryInfo[] {
        //new DirectoryInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFiles), "ffmpeg")),
        //new DirectoryInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFilesX86), "ffmpeg")),
        //new DirectoryInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "ffmpeg")),
        //new DirectoryInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "ffmpeg")),
        //new DirectoryInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "ffmpeg")),
        //new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile))
    };
    private readonly static LeastRecentlyUsedDictionary<FileInfo, IDictionary<string, string>> infoCache = new LeastRecentlyUsedDictionary<FileInfo, IDictionary<string, string>>(500);
    private static readonly Regex RegLine = new Regex(@"^(?:ID|META)_([\w\d_]+)=(.+)$", RegexOptions.Compiled);

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Fidentify")]
    public static readonly string FFidentifyExecutable = FindExecutable("ffidentify");
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Fmpeg")]
    public static readonly string FFmpegExecutable = FindExecutable("ffmpeg");


    public static Size GetFileDimensions(FileInfo file)
    {
      string sw, sh;
      int w, h;
      if (IdentifyFile(file).TryGetValue("VIDEO_WIDTH", out sw)
        && IdentifyFile(file).TryGetValue("VIDEO_HEIGHT", out sh)
        && int.TryParse(sw, out w)
        && int.TryParse(sh, out h)
        && w > 0 && h > 0) {
        return new Size(w, h);
      }
      throw new NotSupportedException();
    }

    public static double GetFileDuration(FileInfo file)
    {
      string sl;
      double dur;
      if (FFmpeg.IdentifyFile(file).TryGetValue("LENGTH", out sl)
        && Double.TryParse(sl, NumberStyles.AllowDecimalPoint, CultureInfo.GetCultureInfo("en-US", "en"), out dur)
        && dur > 0) {
        return dur;
      }
      throw new NotSupportedException();
    }

    public static IDictionary<string, string> IdentifyFile(FileInfo file)
    {
      if (FFmpeg.FFidentifyExecutable == null) {
        throw new NotSupportedException();
      }
      if (file == null) {
        throw new ArgumentNullException("file");
      }
      IDictionary<string, string> rv;
      if (infoCache.TryGetValue(file, out rv)) {
        return rv;
      }
      lock (infoCache) {
        try {
          using (var p = new Process()) {
            var sti = p.StartInfo;
#if !DEBUG
            sti.CreateNoWindow = true;
#endif
            sti.UseShellExecute = false;
            sti.FileName = FFmpeg.FFidentifyExecutable;
            sti.Arguments = String.Format("\"{0}\"", file.FullName);
            sti.LoadUserProfile = false;
            sti.RedirectStandardOutput = true;
            p.Start();
            if (p.WaitForExit(2000) && p.ExitCode == 0) {
              rv = new Dictionary<string, string>();
              string line;
              for (line = p.StandardOutput.ReadLine(); line != null; line = p.StandardOutput.ReadLine()) {
                var m = RegLine.Match(line.Trim());
                if (m.Success) {
                  rv.Add(m.Groups[1].Value, m.Groups[2].Value);
                }
              }
              line = p.StandardOutput.ReadToEnd();
              if (line != null) {
                var m = RegLine.Match(line.Trim());
                if (m.Success) {
                  rv.Add(m.Groups[1].Value, m.Groups[2].Value);
                }
              }
              infoCache.Add(file, rv);
              return rv;
            }
          }
        }
        catch (Exception ex) {
          throw new NotSupportedException(ex.Message, ex);
        }
      }
      throw new NotSupportedException();
    }

    private static string FindExecutable(string executable)
    {
      var isWin = Environment.OSVersion.Platform.ToString().ToLower().Contains("win");
      if (isWin) {
        executable += ".exe";
      }
      List<DirectoryInfo> places = new List<DirectoryInfo>();
      places.Add(new FileInfo(Assembly.GetExecutingAssembly().Location).Directory);
      try {
        places.Add(new DirectoryInfo(Environment.GetEnvironmentVariable("FFMPEG_HOME")));
      }
      catch (Exception) {
      }
      foreach (var l in specialLocations) {
        try {
          places.Add(l);
        }
        catch (Exception) {
          continue;
        }
      }
      foreach (var p in Environment.GetEnvironmentVariable("PATH").Split(isWin ? ';' : ':')) {
        try {
          places.Add(new DirectoryInfo(p.Trim()));
        }
        catch (Exception) {
          continue;
        }
      }

      foreach (var i in places) {
        if (!i.Exists) {
          continue;
        }
        foreach (var di in new[] { i, new DirectoryInfo(Path.Combine(i.FullName, "bin")) }) {
          try {
            var r = di.GetFiles(executable, SearchOption.TopDirectoryOnly);
            if (r.Length != 0) {
              var rv = r[0];
              //LogManager.GetLogger(typeof(FFmpeg)).InfoFormat("Found {0} at {1}", executable, rv.FullName);
              return rv.FullName;
            }
          }
          catch (Exception) {
            continue;
          }
        }
      }
      //LogManager.GetLogger(typeof(FFmpeg)).Warn("Did not find " + executable);
      return null;
    }
  }
}
