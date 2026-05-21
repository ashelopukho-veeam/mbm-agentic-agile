// using System.Collections.Generic;
// using FluentValidation;
// using JetBrains.Annotations;
//
// namespace mbt.webapi.Endpoints;
//
// public class BulkObjectIdRequest
// {
//     public List<string> Ids { get; set; }
// }
//
// [UsedImplicitly]
// public class BulkObjectIdRequestValidator : AbstractValidator<BulkObjectIdRequest>
// {
//     public BulkObjectIdRequestValidator()
//     {
//         RuleFor(v => v.Ids)
//             .NotEmpty()
//             .ForEach(s => s.NotEmpty().Length(24));
//     }
// }



