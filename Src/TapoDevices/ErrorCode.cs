using System.ComponentModel;

namespace TapoDevices
{
    enum ErrorCode : int
    {
        [Description("Success")]
        Success = 0,

        [Description("Secure passthrough protocol deperecated for KLAP")]
        SecurePassthroughDepreacted = 1003,

        [Description("Incorrect request")]
        IncorrectRequest = -1002,

        [Description("JSON formatting error")]
        JsonFormattingError = -1003,

        [Description("Incorrect parameter value")]
        IncorrectParameterValue = -1008,

        [Description("Invalid public key length")]
        InvalidPublicKeyLength = -1010,

        [Description("Invalid terminal UUID")]
        InvalidTerminalUUID = -1012,

        [Description("Invalid request or credentials")]
        InvalidRequestOrCredentials = -1501,

        [Description("No more countdown rules can be added")]
        NoMoreCountdownRules = -1802,

        [Description("Handshake error")]
        HandshakeError = -40401,
    }
}
