//
//  QR Code libray. http://platform.twit88.com/  http://www.codeproject.com/Articles/20574/Open-Source-QRCode-Library
//

using System;
namespace ThoughtWorks.QRCode.ExceptionHandler
{
	
	// Possible Exceptions
	//
	//DecodingFailedException
	//- SymbolNotFoundException
	//  - FinderPatternNotFoundException
	//  - AlignmentPatternNotFoundException
	//- SymbolDataErrorException
	//  - IllegalDataBlockException
	//	- InvalidVersionInfoException
	//- UnsupportedVersionException
	
	//[Serializable]
	public class DecodingFailedException:System.ArgumentException
	{
        internal String message = null;

		public override String Message
		{
			get
			{
				return message;
			}
			
		}
		
		public DecodingFailedException(String message)
		{
			this.message = message;
		}
	}
}