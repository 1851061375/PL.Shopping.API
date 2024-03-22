namespace TD.WebApi.Application.Common.VietQR;

public interface IVietQRService : ITransientService
{
    Task<CreateQRBankResponse>? GetVietQR(int orderCode, int amount, string description, string cancelUrl, string returnUrl, int expiredAt);
}