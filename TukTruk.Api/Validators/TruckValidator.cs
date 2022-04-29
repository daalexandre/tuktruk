using FluentValidation;
using TukTruk.Api.Enums;
using TukTruk.Api.Models;

public class TruckValidator : AbstractValidator<Truck>
{
    public TruckValidator()
    {
        RuleFor(truck => truck.Model)
        .IsInEnum().WithMessage("O modelo é inválido");

        RuleFor(truck => truck.ManufacturingYear).Equal(DateTime.Now.Year).WithMessage("O ano de fabricação não pode ser menor que o ano atual");

        RuleFor(truck => truck.ModelYear)
        .GreaterThanOrEqualTo(truck => truck.ManufacturingYear)
        .WithMessage("O ano do modelo não pode ser menor que o ano de fabricaçãso")
        .LessThanOrEqualTo(truck => truck.ManufacturingYear + 1)
        .WithMessage("O ano do modelo igual ou subequente ao ano de fabricação.");
    }
}