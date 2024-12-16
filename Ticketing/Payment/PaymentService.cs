using SourceGenerator;

[RegisterService(typeof(IPaymentService), LifeTime.Scoped)]
public class PaymentService : IPaymentService
{
    public string ProcessPayment(PaymentRequest payment)
    {
        return $"Payment of {payment.id} {payment} processed for card {payment}.";
    }
}
