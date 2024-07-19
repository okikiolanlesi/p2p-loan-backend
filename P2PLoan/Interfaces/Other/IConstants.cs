namespace P2PLoan.Interfaces;

public interface IConstants
{
    public int MAX_LOAN_AMOUNT { get; }
    public string SYSTEM_USER_EMAIL { get; }
    public string SYSTEM_USER_FIRST_NAME { get; }
    public string SYSTEM_USER_LAST_NAME { get; }
    public int PASSWORD_RESET_TOKEN_EXPIRATION_MINUTES { get; }
    public int PASSWORD_RESET_TOKEN_LENGTH { get; }
    public int EMAIL_VERIFICATION_TOKEN_LENGTH { get; }
    public int EMAIL_VERIFICATION_TOKEN_EXPIRATION_MINUTES { get; }
}