using System.Collections.Generic;

namespace AutoRentimine.BlazorWasm.Api
{
    public class OperationResult
    {
        public Dictionary<string, string> PropertyErrors { get; set; } = new();
        public List<string> Errors { get; set; } = new();

        public bool HasErrors => PropertyErrors?.Count > 0 || Errors?.Count > 0;

        public OperationResult AddError(string error)
        {
            Errors.Add(error);
            return this;
        }
    }

    public class OperationResult<T> : OperationResult
    {
        public T? Value { get; set; }

        public OperationResult() { }
        public OperationResult(T value)
        {
            Value = value;
        }
    }
}