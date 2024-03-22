namespace TD.WebApi.Application.Common.Models;

public class ResultVietQR : IResultVietQR
{
    public ResultVietQR()
    {
    }

    public bool Success { get; set; }

    public static IResultVietQR Done(bool success)
    {
        return new ResultVietQR { Success = success};
    }
}

public class ResultVietQR<T> : ResultVietQR, IResultVietQR<T>
{
    public ResultVietQR()
    {
    }

    public T Data { get; set; }

    public static new ResultVietQR<T> Done(bool success)
    {
        return new ResultVietQR<T> { Success = success};
    }

    public static ResultVietQR<T> Done(T data, bool success)
    {
        return new ResultVietQR<T> { Success = success, Data = data};
    }

}