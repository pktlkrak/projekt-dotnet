using System.ComponentModel.DataAnnotations;
using ClinicManager.Dtos.Medications;
using ClinicManager.Dtos.Patients;
using ClinicManager.Dtos.Visits;

namespace ClinicManagerTests;

[TestFixture]
public class DtoValidationTests
{
    private static List<ValidationResult> Validate(object obj)
    {
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(obj, new ValidationContext(obj), results, validateAllProperties: true);
        return results;
    }

    private static bool IsValid(object obj) => Validate(obj).Count == 0;

    // --- ProcedureRefDto ---

    [Test]
    public void ProcedureRefDto_ZeroProcedureId_FailsValidation()
    {
        var dto = new ProcedureRefDto { ProcedureId = 0, Cost = 100 };
        Assert.That(IsValid(dto), Is.False);
    }

    [Test]
    public void ProcedureRefDto_NegativeCost_FailsValidation()
    {
        var dto = new ProcedureRefDto { ProcedureId = 1, Cost = -0.01 };
        Assert.That(IsValid(dto), Is.False);
    }

    [Test]
    public void ProcedureRefDto_ValidData_PassesValidation()
    {
        var dto = new ProcedureRefDto { ProcedureId = 1, Cost = 0 };
        Assert.That(IsValid(dto), Is.True);
    }

    [Test]
    public void ProcedureRefDto_ZeroCostIsAllowed()
    {
        var dto = new ProcedureRefDto { ProcedureId = 5, Cost = 0.0 };
        Assert.That(IsValid(dto), Is.True);
    }

    // --- MedicationFormDto ---

    [Test]
    public void MedicationFormDto_EmptyName_FailsValidation()
    {
        var dto = new MedicationFormDto { Name = "", Cost = 10 };
        Assert.That(IsValid(dto), Is.False);
    }

    [Test]
    public void MedicationFormDto_NegativeCost_FailsValidation()
    {
        var dto = new MedicationFormDto { Name = "Ibuprofen", Cost = -1 };
        Assert.That(IsValid(dto), Is.False);
    }

    [Test]
    public void MedicationFormDto_ValidData_PassesValidation()
    {
        var dto = new MedicationFormDto { Name = "Ibuprofen", Cost = 9.99 };
        Assert.That(IsValid(dto), Is.True);
    }

    // --- PatientFormDto ---

    [Test]
    public void PatientFormDto_InvalidPesel_FailsValidation()
    {
        var dto = ValidPatient();
        dto.Pesel = "123";
        Assert.That(IsValid(dto), Is.False);
    }

    [Test]
    public void PatientFormDto_InvalidEmail_FailsValidation()
    {
        var dto = ValidPatient();
        dto.Email = "not-an-email";
        Assert.That(IsValid(dto), Is.False);
    }

    [Test]
    public void PatientFormDto_ShortPhoneNumber_FailsValidation()
    {
        var dto = ValidPatient();
        dto.PhoneNumber = "12345";
        Assert.That(IsValid(dto), Is.False);
    }

    [Test]
    public void PatientFormDto_ValidData_PassesValidation()
    {
        Assert.That(IsValid(ValidPatient()), Is.True);
    }

    // --- VisitCreateDto ---

    [Test]
    public void VisitCreateDto_EmptyDoctorId_FailsValidation()
    {
        var dto = new VisitCreateDto { DoctorId = "", PatientId = 1, ScheduledAt = DateTime.Now };
        Assert.That(IsValid(dto), Is.False);
    }

    [Test]
    public void VisitCreateDto_ValidData_PassesValidation()
    {
        var dto = new VisitCreateDto { DoctorId = "abc-123", PatientId = 1, ScheduledAt = DateTime.Now };
        Assert.That(IsValid(dto), Is.True);
    }

    private static PatientFormDto ValidPatient() => new()
    {
        FirstName     = "Jan",
        LastName      = "Kowalski",
        Pesel         = "85061512345",
        PhoneNumber   = "123456789",
        Email         = "jan.kowalski@example.com",
        DateOfBirth   = new DateOnly(1985, 6, 15),
        InsuranceNumber = "INS-001",
    };
}
