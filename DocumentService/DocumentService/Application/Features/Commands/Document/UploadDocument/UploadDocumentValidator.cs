using FileSignatures.Formats;
using FluentValidation;

namespace DocumentService.Application.Features.Commands.Document.UploadDocument
{
    public class UploadDocumentValidator : AbstractValidator<UploadDocumentCommand>
    {
        private static readonly string[] AllowedExtensions = [".pdf", ".docx", ".txt", ".csv", ".xlsx"];

        public UploadDocumentValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty();

            RuleFor(x => x.file)
                .NotNull()
                .WithMessage("File can`t be empty");

            RuleFor(x => x.file.Length)
                .GreaterThan(0)
                .LessThanOrEqualTo(50 * 1024 * 1024)
                .WithMessage("Size file can`t be more then 50 MB");

            RuleFor(x => x.file.FileName)
                .Must(fileName => AllowedExtensions.Contains(Path.GetExtension(fileName).ToLowerInvariant()))
                .WithMessage("Upsupported format of file");
        }
    }
}
