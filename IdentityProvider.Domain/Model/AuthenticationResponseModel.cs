namespace IdentityProvider.Domain.Model;

	public class AuthenticationResponseModel
	{
		public bool IsError { get; set; }
		public SmartIdSuccessResponse? SuccessResponse { get; set; }
		public SmartIdErrorResponse? ErrorResponse { get; set; }
	}
	
	public class SmartIdSuccessResponse
	{
		public string? SessionId { get; set; } 
		public string? VerificationCode { get; set; } 
	}
	
	public class SmartIdErrorResponse
	{
		public string? Type { get; set; } 
		public string? Title { get; set; } 
		public int Status { get; set; }
		public string? Detail { get; set; } 
		public object? Instance { get; set; } 
		public object? Properties { get; set; }
		public int Code { get; set; }
		public string? Message { get; set; } 

	}
