//
//  QR Code libray. http://platform.twit88.com/  http://www.codeproject.com/Articles/20574/Open-Source-QRCode-Library
//

using System;
namespace ThoughtWorks.QRCode.ExceptionHandler
{
	//[Serializable]
	public class InvalidVersionException:VersionInformationException
	{
        internal String message;
		public override String Message
		{
			get
			{
				return message;
			}
			
		}
		
		public InvalidVersionException(String message)
		{
			this.message = message;
		}
	}
}