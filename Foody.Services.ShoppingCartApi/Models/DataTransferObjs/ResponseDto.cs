namespace Foody.Services.ShoppingCartApi.Models.DataTransferObjs
{
    public class ResponseDto
    {
        public bool IsSuccess { get; set; } = true;
        public object Result { get; set; }

        public string DisplayMessage { get; set; } = string.Empty;
        public List<string> ErrorMessage { get; set; }
    }
}
