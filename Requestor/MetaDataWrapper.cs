namespace Requestor
{
    public class MetaDataWrapper
    {
        public string message;
        public string operation_id;

        public MetaDataWrapper(string message, string operation_id)
        {
            this.message = message;
            this.operation_id = operation_id;
        }
    }
}