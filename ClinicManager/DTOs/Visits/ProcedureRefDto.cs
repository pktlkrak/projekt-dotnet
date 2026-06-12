using System.ComponentModel.DataAnnotations;
using ClinicManager.Utils;
using Microsoft.AspNetCore.Mvc;

namespace ClinicManager.Dtos.Visits
{
    public class ProcedureRefDto
    {
        [Range(1, int.MaxValue, ErrorMessage = "Please select a procedure.")]
        public int ProcedureId { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Cost must be non-negative.")]
        [ModelBinder(BinderType = typeof(InvariantDoubleModelBinder))]
        public double Cost { get; set; }
    }
}
