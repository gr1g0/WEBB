using Microsoft.AspNetCore.Http.HttpResults;
using Moq;

public class AuthTests
{
    private readonly Mock<IDBManager> mockDbManager;
    private readonly AuthRequests authRequests;

    public AuthTests()
    {
        mockDbManager = new Mock<IDBManager>();
        authRequests = new AuthRequests(mockDbManager.Object);
    }
    
    [Fact]
    public void SignUpValidRequestReturnsOkResult()
    {
        string Login = "testuser"; 
        string Password = "testpass";
        mockDbManager.Setup(x => x.AddUser(Login, Password)).Returns(true);
        var result = authRequests.SignUp(Login, Password);
        var okResult = Assert.IsType<Ok<string>>(result);
    }

    [Fact]
    public void SignUpInvalidRequestReturnsProblem()
    {
        string Login = "testuser"; 
        string Password = "testpass";
        mockDbManager.Setup(x => x.AddUser(Login, Password)).Returns(false);
        var result = authRequests.SignUp(Login, Password);
        var badRequestResult = Assert.IsType<ProblemHttpResult>(result);
    }

    [Fact]
    public void LoginInvalidCredentialsReturnsUnauthorizedResult()
    {
        string Login = "testuser"; 
        string Password = "testpass";
        mockDbManager.Setup(x => x.CheckUser(Login, Password)).Returns(false);
        var result = authRequests.Login(Login, Password);
        var unauthorizedResult = Assert.IsType<UnauthorizedHttpResult>(result);
    }

    [Fact]
    public void DeleteUserValidRequestReturnsOkResult()
    {
        string Login = "testuser";
        string Password = "testpass";
        mockDbManager.Setup(x => x.DeleteUser(Login)).Returns(true);
        mockDbManager.Setup(x => x.CheckUser(Login, Password)).Returns(true);
        var result = authRequests.DelUser(Login, Password);
        var okResult = Assert.IsType<Ok<string>>(result);
    }
    [Fact]
    public void ChangePasswordValidRequestReturnsOkResult()
    {
        string Login = "testuser";
        string OldPassword = "oldpass";
        string NewPassword = "newpass";
        mockDbManager.Setup(x => x.CheckUser(Login, OldPassword)).Returns(true);
        mockDbManager.Setup(x => x.ChangePassword(Login, NewPassword)).Returns(true);
        var result = authRequests.ChangePass(Login, OldPassword, NewPassword);
        var okResult = Assert.IsType<Ok<string>>(result);
    }

    [Fact]
    public void ChangePasswordSameOldAndNewPasswordReturnsBadRequest()
    {
        string Login = "testuser";
        string OldPassword = "samepass";
        string NewPassword = "samepass";
        mockDbManager.Setup(x => x.CheckUser(Login, OldPassword)).Returns(true);
        var result = authRequests.ChangePass(Login, OldPassword, NewPassword);
        var badRequestResult = Assert.IsType<BadRequest<string>>(result);
    }
}