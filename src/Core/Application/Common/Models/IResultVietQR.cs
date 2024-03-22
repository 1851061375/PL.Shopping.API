namespace TD.WebApi.Application.Common.Models;

public interface IResultVietQR
{
    bool Success { get; set; }

}

public interface IResultVietQR<out T> : IResultVietQR
{
    T Data { get; }
}