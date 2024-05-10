using System;

namespace Ursa.Music._163.Utils;

public class MciException : Exception
{
    private MciError err;

    public MciException()
    {
    }

    public MciException(int errorId)
    {
        if (Enum.IsDefined(typeof(MciError), errorId))
            err = (MciError)errorId;
        else
            throw new ArgumentOutOfRangeException("不是正确的错误ID");
    }

    public MciException(string errorName)
    {
        if (!Enum.TryParse<MciError>(errorName, out err))
            throw new ArgumentOutOfRangeException("不是正确的错误ID");
    }

    public int ErrorId
    {
        get => (int)err;
    }

    public string ErrorName
    {
        get => err.ToString();
    }

    public override string Message
    {
        get => ErrorName;
    }

    enum MciError
    {
        MCIERR_NO_ERROR = 0,

        MCIERR_INVALID_DEVICE_ID = 257,
        MCIERR_UNRECOGNIZED_KEYWORD = 259,
        MCIERR_UNRECOGNIZED_COMMAND = 261,
        MCIERR_HARDWARE = 262,
        MCIERR_INVALID_DEVICE_NAME = 263,
        MCIERR_OUT_OF_MEMORY = 264,
        MCIERR_DEVICE_OPEN = 265,
        MCIERR_CANNOT_LOAD_DRIVER = 266,
        MCIERR_MISSING_COMMAND_STRING = 267,
        MCIERR_PARAM_OVERFLOW = 268,
        MCIERR_MISSING_STRING_ARGUMENT = 269,
        MCIERR_BAD_INTEGER = 270,
        MCIERR_PARSER_INTERNAL = 271,
        MCIERR_DRIVER_INTERNAL = 272,
        MCIERR_MISSING_PARAMETER = 273,
        MCIERR_UNSUPPORTED_FUNCTION = 274,
        MCIERR_FILE_NOT_FOUND = 275,
        MCIERR_DEVICE_NOT_READY = 276,
        MCIERR_INTERNAL = 277,
        MCIERR_DRIVER = 278,
        MCIERR_CANNOT_USE_ALL = 279,
        MCIERR_MULTIPLE = 280,
        MCIERR_EXTENSION_NOT_FOUND = 281,
        MCIERR_OUTOFRANGE = 282,
        MCIERR_FLAGS_NOT_COMPATIBLE = 284,
        MCIERR_FILE_NOT_SAVED = 286,
        MCIERR_DEVICE_TYPE_REQUIRED = 287,
        MCIERR_DEVICE_LOCKED = 288,
        MCIERR_DUPLICATE_ALIAS = 289,
        MCIERR_BAD_CONSTANT = 290,
        MCIERR_MUST_USE_SHAREABLE = 291,
        MCIERR_MISSING_DEVICE_NAME = 292,
        MCIERR_BAD_TIME_FORMAT = 293,
        MCIERR_NO_CLOSING_QUOTE = 294,
        MCIERR_DUPLICATE_FLAGS = 295,
        MCIERR_INVALID_FILE = 296,
        MCIERR_NULL_PARAMETER_BLOCK = 297,
        MCIERR_UNNAMED_RESOURCE = 298,
        MCIERR_NEW_REQUIRES_ALIAS = 299,
        MCIERR_NOTIFY_ON_AUTO_OPEN = 300,
        MCIERR_NO_ELEMENT_ALLOWED = 301,
        MCIERR_NONAPPLICABLE_FUNCTION = 302,
        MCIERR_ILLEGAL_FOR_AUTO_OPEN = 303,
        MCIERR_FILENAME_REQUIRED = 304,
        MCIERR_EXTRA_CHARACTERS = 305,
        MCIERR_DEVICE_NOT_INSTALLED = 306,
        MCIERR_GET_CD = 307,
        MCIERR_SET_CD = 308,
        MCIERR_SET_DRIVE = 309,
        MCIERR_DEVICE_LENGTH = 310,
        MCIERR_DEVICE_ORD_LENGTH = 311,
        MCIERR_NO_INTEGER = 312,

        MCIERR_WAVE_OUTPUTSINUSE = 320,
        MCIERR_WAVE_SETOUTPUTINUSE = 321,
        MCIERR_WAVE_INPUTSINUSE = 322,
        MCIERR_WAVE_SETINPUTINUSE = 323,
        MCIERR_WAVE_OUTPUTUNSPECIFIED = 324,
        MCIERR_WAVE_INPUTUNSPECIFIED = 325,
        MCIERR_WAVE_OUTPUTSUNSUITABLE = 326,
        MCIERR_WAVE_SETOUTPUTUNSUITABLE = 327,
        MCIERR_WAVE_INPUTSUNSUITABLE = 328,
        MCIERR_WAVE_SETINPUTUNSUITABLE = 329,

        MCIERR_SEQ_DIV_INCOMPATIBLE = 336,
        MCIERR_SEQ_PORT_INUSE = 337,
        MCIERR_SEQ_PORT_NONEXISTENT = 338,
        MCIERR_SEQ_PORT_MAPNODEVICE = 339,
        MCIERR_SEQ_PORT_MISCERROR = 340,
        MCIERR_SEQ_TIMER = 341,
        MCIERR_SEQ_PORTUNSPECIFIED = 342,
        MCIERR_SEQ_NOMIDIPRESENT = 343,

        MCIERR_NO_WINDOW = 346,
        MCIERR_CREATEWINDOW = 347,
        MCIERR_FILE_READ = 348,
        MCIERR_FILE_WRITE = 349,
        MCIERR_NO_IDENTITY = 350,
    }
}