using System.ComponentModel;

namespace TapoDevices
{
    enum ErrorCode : int
    {
        [Description("Success")]
        Success = 0,

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

        [Description("Handshake error")]
        HandshakeError = -40401,
    }
}
