namespace DocumentService.Core.ValueObjects
{
    public class DocumentName : IEquatable<DocumentName>
    {
        public string Value { get; }
        private DocumentName(string _value) => Value = _value;
        public static DocumentName Create(string value)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentException("Имя файла не может быть пустым.", nameof(value));
            if (value.Length > 255) 
                throw new ArgumentException("Имя файла слишком длинное.", nameof(value));

            return new DocumentName(value);
        }
        public bool Equals(DocumentName? other) => other is not null && Value == other.Value;
        public override bool Equals(object? obj) => obj is DocumentName other && Equals(other);
        public override int GetHashCode() => Value.GetHashCode();
        public override string ToString() => Value;
    }
}
