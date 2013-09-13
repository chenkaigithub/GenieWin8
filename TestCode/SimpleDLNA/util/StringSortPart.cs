using System;

namespace NMaier.SimpleDlna.Utilities
{
  internal sealed class StringSortPart : BaseSortPart, IComparable<StringSortPart>
  {
    private readonly StringComparer comparer;

    private readonly string str;


    public StringSortPart(string str, StringComparer comparer)
    {
      this.str = str;
      this.comparer = comparer;
    }


    public int CompareTo(StringSortPart other)
    {
      if (other == null) {
        throw new ArgumentNullException("other");
      }
      return comparer.Compare(str, other.str);
    }
  }
}
