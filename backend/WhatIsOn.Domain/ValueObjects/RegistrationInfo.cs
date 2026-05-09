namespace WhatIsOn.Domain.ValueObjects;

public class RegistrationInfo
{
    public DateOnly OpenDate { get; set; }
    public DateOnly CloseDate { get; set; }
    public decimal Fee { get; set; }
    public decimal EarlyBirdDiscount { get; set; }
}
