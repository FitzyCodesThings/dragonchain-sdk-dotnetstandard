using System;

namespace DragonchainSDK.Framework.Errors
{
    public class FailureByDesignException : Exception
    {
        public FailureCode Code { get; set; }

        public FailureByDesignException(FailureCode code, string message = "Failure By Design")
            :base(message)
        {
            Code = code;
        }
    }

    public enum FailureCode
    {
        PARAM_ERROR,
        NOT_FOUND
    }
}