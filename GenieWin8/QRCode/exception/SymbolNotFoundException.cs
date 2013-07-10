//
//  QR Code libray. http://platform.twit88.com/  http://www.codeproject.com/Articles/20574/Open-Source-QRCode-Library
//

using System;
namespace ThoughtWorks.QRCode.ExceptionHandler
{
	//[Serializable]
	public class SymbolNotFoundException:System.ArgumentException
	{
        internal String message = null;

		public override String Message
		{
			get
			{
				return message;
			}
			
		}
		
		public SymbolNotFoundException(String message)
		{
			this.message = message;
		}
	}
}