namespace eTurns.DTO
{
    public enum MessageCode
    {
        OK = 1,
        ERROR = 2,
        UNAUTHENTICATE = 3
    }

    public class MessageDTO
    {
        public string Message { get; set; }
        public MessageCode Code { get; set; }
        public bool Success { get; set; }

        public void SetMessage(string message, MessageCode msgCode, bool SuccessMessage)
        {
            this.Message = message;
            this.Success = SuccessMessage;
            this.Code = msgCode;
        }

        public void SetSuccessMessage(string message)
        {
            this.Message = message;
            this.Success = true;
            this.Code = MessageCode.OK;
        }

        public void SetErrorMessage(string message)
        {
            this.Message = message;
            this.Success = false;
            this.Code = MessageCode.ERROR;
        }
    }

    public class InsertMessage : MessageDTO
    {
        public int ID { get; set; }
    }


}
