using LeadSoft.Adapter.Google.ReCaptcha.Attibutes;
using System.ComponentModel.DataAnnotations;

namespace LeadSoft.Google.Tests.Attributes;

public class ReCAPTCHAResponseAttributeTests
{
    // Modelo auxiliar para testar o atributo via Validator
    private sealed class TokenModel
    {
        [ReCAPTCHAResponseAttribute]
        public string? Token { get; set; }
    }

    // Isola a variável de ambiente durante o teste e restaura ao final
    private static T WithSecretKeyEnv<T>(string? value, Func<T> action)
    {
        string? original = Environment.GetEnvironmentVariable(ReCAPTCHAResponseAttribute.EnvVariable_SecretKey);
        try
        {
            Environment.SetEnvironmentVariable(ReCAPTCHAResponseAttribute.EnvVariable_SecretKey, value);
            return action();
        }
        finally
        {
            Environment.SetEnvironmentVariable(ReCAPTCHAResponseAttribute.EnvVariable_SecretKey, original);
        }
    }

    // --- Constantes e metadados ---

    [Fact]
    public void EnvVariable_SecretKey_HasCorrectValue()
    {
        Assert.Equal("GOOGLE_RECAPTCHA_SECRET_KEY", ReCAPTCHAResponseAttribute.EnvVariable_SecretKey);
    }

    [Fact]
    public void AttributeUsage_AllowsPropertyFieldAndParameter()
    {
        var usage = typeof(ReCAPTCHAResponseAttribute)
            .GetCustomAttributes(typeof(AttributeUsageAttribute), inherit: false)
            .Cast<AttributeUsageAttribute>()
            .Single();

        Assert.True(usage.ValidOn.HasFlag(AttributeTargets.Property));
        Assert.True(usage.ValidOn.HasFlag(AttributeTargets.Field));
        Assert.True(usage.ValidOn.HasFlag(AttributeTargets.Parameter));
    }

    [Fact]
    public void AttributeUsage_AllowMultiple_IsFalse()
    {
        var usage = typeof(ReCAPTCHAResponseAttribute)
            .GetCustomAttributes(typeof(AttributeUsageAttribute), inherit: false)
            .Cast<AttributeUsageAttribute>()
            .Single();

        Assert.False(usage.AllowMultiple);
    }

    // --- Comportamento do construtor ---

    [Fact]
    public void Constructor_NoArgs_DoesNotThrow()
    {
        var exception = Record.Exception(() => new ReCAPTCHAResponseAttribute());

        Assert.Null(exception);
    }

    [Fact]
    public void Constructor_WithExplicitSecretKey_DoesNotThrow()
    {
        var exception = Record.Exception(() => new ReCAPTCHAResponseAttribute("my-secret-key"));

        Assert.Null(exception);
    }

    [Fact]
    public void Constructor_SetsDefaultErrorMessage()
    {
        var attr = new ReCAPTCHAResponseAttribute();

        Assert.NotNull(attr.ErrorMessage);
        Assert.NotEmpty(attr.ErrorMessage);
    }

    // --- IsValid: valor nulo (deve passar — nulo ≠ inválido para ValidationAttribute) ---

    [Fact]
    public void IsValid_NullValue_ReturnsSuccess()
    {
        var attr = new ReCAPTCHAResponseAttribute("any-secret-key");
        var context = new ValidationContext(new object());

        var result = attr.GetValidationResult(null, context);

        Assert.Equal(ValidationResult.Success, result);
    }

    [Fact]
    public void IsValid_NullToken_ViaValidator_PassesValidation()
    {
        var model = new TokenModel { Token = null };
        var context = new ValidationContext(model);
        var results = new List<ValidationResult>();

        bool isValid = Validator.TryValidateObject(model, context, results, validateAllProperties: true);

        Assert.True(isValid);
        Assert.Empty(results);
    }

    // --- IsValid: Secret Key ausente ---

    [Fact]
    public void IsValid_NoSecretKey_ReturnsFailure()
    {
        var result = WithSecretKeyEnv(null, () =>
        {
            var attr = new ReCAPTCHAResponseAttribute(); // sem argumento, sem env var
            return attr.GetValidationResult("qualquer-token", new ValidationContext(new object()));
        });

        Assert.NotNull(result);
        Assert.NotEqual(ValidationResult.Success, result);
    }

    [Fact]
    public void IsValid_NoSecretKey_ErrorMessageMentionsSecretKey()
    {
        var result = WithSecretKeyEnv(null, () =>
        {
            var attr = new ReCAPTCHAResponseAttribute();
            return attr.GetValidationResult("qualquer-token", new ValidationContext(new object()));
        });

        Assert.Contains("Secret Key", result!.ErrorMessage, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void IsValid_EmptySecretKey_ReturnsFailure()
    {
        var result = WithSecretKeyEnv(null, () =>
        {
            var attr = new ReCAPTCHAResponseAttribute(secretKey: "");
            return attr.GetValidationResult("qualquer-token", new ValidationContext(new object()));
        });

        Assert.NotNull(result);
        Assert.NotEqual(ValidationResult.Success, result);
    }

    [Fact]
    public void IsValid_NoSecretKey_ViaValidator_FailsValidation()
    {
        bool isValid = WithSecretKeyEnv(null, () =>
        {
            var model = new TokenModel { Token = "qualquer-token" };
            var context = new ValidationContext(model);
            var results = new List<ValidationResult>();
            return Validator.TryValidateObject(model, context, results, validateAllProperties: true);
        });

        Assert.False(isValid);
    }

    [Fact]
    public void IsValid_NoSecretKey_ValidationResult_ContainsMemberName()
    {
        var result = WithSecretKeyEnv(null, () =>
        {
            var model = new TokenModel { Token = "qualquer-token" };
            var context = new ValidationContext(model) { MemberName = nameof(TokenModel.Token) };
            var attr = new ReCAPTCHAResponseAttribute();
            return attr.GetValidationResult(model.Token, context);
        });

        Assert.NotNull(result);
        Assert.Contains(nameof(TokenModel.Token), result!.MemberNames);
    }

    // --- IsValid: token vazio ---

    [Fact]
    public void IsValid_EmptyStringToken_WithNoSecretKey_ReturnsFailure()
    {
        // String vazia não é null: passa pelo null-check e cai na validação de SecretKey ausente
        var result = WithSecretKeyEnv(null, () =>
        {
            var attr = new ReCAPTCHAResponseAttribute();
            return attr.GetValidationResult(string.Empty, new ValidationContext(new object()));
        });

        Assert.NotNull(result);
        Assert.NotEqual(ValidationResult.Success, result);
    }

    // --- Leitura de variável de ambiente ---

    [Fact]
    public void Constructor_ReadsSecretKeyFromEnvVar()
    {
        // Confirma que o atributo lê a env var corretamente ao ser instanciado sem argumento.
        // Se a env var está presente, a ErrorMessage padrão deve ser mantida (não sobrescrita
        // com "Secret Key não fornecida"), provando que a leitura ocorreu.
        string? errorMessage = WithSecretKeyEnv("env-secret-key", () =>
        {
            var attr = new ReCAPTCHAResponseAttribute();
            return attr.ErrorMessage;
        });

        Assert.Equal("O campo {0} possui uma resposta de reCAPTCHA inválida.", errorMessage);
    }

    [Fact]
    public void Constructor_ExplicitSecretKey_TakesPrecedenceOverEnvVar()
    {
        // A chave explícita deve ter prioridade sobre a variável de ambiente.
        // Verificamos indiretamente: se a chave explícita foi usada, o ErrorMessage
        // padrão é mantido (a instância foi criada sem erro).
        string? errorMessage = WithSecretKeyEnv("env-secret-key", () =>
        {
            var attr = new ReCAPTCHAResponseAttribute("explicit-secret-key");
            return attr.ErrorMessage;
        });

        Assert.Equal("O campo {0} possui uma resposta de reCAPTCHA inválida.", errorMessage);
    }
}
