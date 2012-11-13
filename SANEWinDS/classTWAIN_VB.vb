'
'   Copyright 2011, 2012 Alec Skelly
'
'   This file is part of SANEWinDS.
'
'   SANEWinDS is free software: you can redistribute it and/or modify
'   it under the terms of the GNU General Public License as published by
'   the Free Software Foundation, either version 3 of the License, or
'   (at your option) any later version.
'
'   SANEWinDS is distributed in the hope that it will be useful,
'   but WITHOUT ANY WARRANTY; without even the implied warranty of
'   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
'   GNU General Public License for more details.
'
'   You should have received a copy of the GNU General Public License
'   along with SANEWinDS.  If not, see <http://www.gnu.org/licenses/>.
'
Imports System.Runtime.InteropServices
Namespace TWAIN_VB

    ' /****************************************************************************
    ' * Structure Definitions                                                    *
    ' ****************************************************************************/

    '/* Fixed point structure type. */
    <StructLayout(LayoutKind.Sequential, Pack:=2, CharSet:=CharSet.Ansi)> _
    Public Structure TW_FIX32
        Dim Whole As Int16        '/* maintains the sign */
        Dim Frac As UInt16
    End Structure

    '/* No DAT.  Defines a frame rectangle in ICAP_UNITS coordinates. */
    <StructLayout(LayoutKind.Sequential, Pack:=2, CharSet:=CharSet.Ansi)> _
    Public Structure TW_FRAME
        Dim Left As TW_FIX32
        Dim Top As TW_FIX32
        Dim Right As TW_FIX32
        Dim Bottom As TW_FIX32
    End Structure

    '/* No DAT needed. */
    'typedef struct {
    '   TW_FIX32   StartIn;
    '   TW_FIX32   BreakIn;
    '   TW_FIX32   EndIn;
    '   TW_FIX32   StartOut;
    '   TW_FIX32   BreakOut;
    '   TW_FIX32   EndOut;
    '   TW_FIX32   Gamma;
    '   TW_FIX32   SampleCount;  /* if =0 use the gamma */
    '} TW_DECODEFUNCTION, FAR * pTW_DECODEFUNCTION;

    '/* No DAT needed. */
    'typedef struct {
    '   TW_DECODEFUNCTION   Decode[3];
    '   TW_FIX32            Mix[3][3];
    '} TW_TRANSFORMSTAGE, FAR * pTW_TRANSFORMSTAGE;

    '/* TWON_ARRAY. Container for array of values (a simplified TW_ENUMERATION) */
    <StructLayout(LayoutKind.Sequential, Pack:=2, CharSet:=CharSet.Ansi)> _
    Public Structure TW_ARRAY
        Dim ItemType As UInt16
        Dim NumItems As UInt32      '/* How many items in ItemList           */
        Dim ItemList() As Byte      '/* Array of ItemType values starts here */
    End Structure

    '/* DAT_AUDIOINFO, information about audio data */
    'typedef struct {
    '   TW_STR255  Name;       /* name of audio data */
    '   TW_UINT32  Reserved;   /* reserved space */
    '} TW_AUDIOINFO, FAR * pTW_AUDIOINFO;

    '/* TW_CALLBACK, used to register callbacks Added 2.0 */
    'typedef struct  {
    '    TW_MEMREF   CallBackProc;
    '    TW_UINT32   RefCon;
    '    TW_INT16    Message;
    '} TW_CALLBACK, FAR * pTW_CALLBACK;

    '/* DAT_CAPABILITY. Used by application to get/set capability from/in a data source. */
    <StructLayout(LayoutKind.Sequential, Pack:=2, CharSet:=CharSet.Ansi)> _
    Public Structure TW_CAPABILITY
        Dim Cap As UInt16           '/* id of capability to set or get, e.g. CAP_BRIGHTNESS */
        Dim ConType As UInt16       '/* TWON_ONEVALUE, _RANGE, _ENUMERATION or _ARRAY   */
        Dim hContainer As IntPtr    '/* Handle to container of type Dat              */
    End Structure

    '/* No DAT needed. */
    'typedef struct {
    '   TW_FIX32   X;
    '   TW_FIX32   Y;
    '   TW_FIX32   Z;
    '} TW_CIEPOINT, FAR * pTW_CIEPOINT;

    '/* DAT_CIECOLOR. */
    'typedef struct {
    '   TW_UINT16           ColorSpace;
    '   TW_INT16            LowEndian;
    '   TW_INT16            DeviceDependent;
    '   TW_INT32            VersionNumber;
    '   TW_TRANSFORMSTAGE   StageABC;
    '   TW_TRANSFORMSTAGE   StageLMN;
    '   TW_CIEPOINT         WhitePoint;
    '   TW_CIEPOINT         BlackPoint;
    '   TW_CIEPOINT         WhitePaper;
    '   TW_CIEPOINT         BlackInk;
    '   TW_FIX32            Samples[1];
    '} TW_CIECOLOR, FAR * pTW_CIECOLOR;

    <StructLayout(LayoutKind.Sequential, Pack:=2, CharSet:=CharSet.Ansi)> _
    Public Structure TW_CUSTOMDSDATA
        Dim InfoLength As UInt32     '/* Length of Information in bytes.  */
        Dim hData As IntPtr          '/* Place holder for data, DS Allocates */
    End Structure

    '/* DAT_DEVICEEVENT, information about events */
    'typedef struct {
    '   TW_UINT32  Event;                  /* One of the TWDE_xxxx values. */
    '   TW_STR255  DeviceName;             /* The name of the device that generated the event */
    '   TW_UINT32  BatteryMinutes;         /* Battery Minutes Remaining    */
    '   TW_INT16   BatteryPercentage;      /* Battery Percentage Remaining */
    '   TW_INT32   PowerSupply;            /* Power Supply                 */
    '   TW_FIX32   XResolution;            /* Resolution                   */
    '   TW_FIX32   YResolution;            /* Resolution                   */
    '   TW_UINT32  FlashUsed2;             /* Flash Used2                  */
    '   TW_UINT32  AutomaticCapture;       /* Automatic Capture            */
    '   TW_UINT32  TimeBeforeFirstCapture; /* Automatic Capture            */
    '   TW_UINT32  TimeBetweenCaptures;    /* Automatic Capture            */
    '} TW_DEVICEEVENT, FAR * pTW_DEVICEEVENT;

    '/* No DAT needed. */
    'typedef struct {
    '   TW_UINT8    Index;    /* Value used to index into the color table. */
    '   TW_UINT8    Channel1; /* First  tri-stimulus value (e.g Red)       */
    '   TW_UINT8    Channel2; /* Second tri-stimulus value (e.g Green)     */
    '   TW_UINT8    Channel3; /* Third  tri-stimulus value (e.g Blue)      */
    '} TW_ELEMENT8, FAR * pTW_ELEMENT8;

    '/* TWON_ENUMERATION. Container for a collection of values. */
    <StructLayout(LayoutKind.Sequential, Pack:=2, CharSet:=CharSet.Ansi)> _
    Public Structure TW_ENUMERATION
        Dim ItemType As UInt16
        Dim NumItems As UInt32      '/* How many items in ItemList                 */
        Dim CurrentIndex As UInt32  '/* Current value is in ItemList[CurrentIndex] */
        Dim DefaultIndex As UInt32  '/* Powerup value is in ItemList[DefaultIndex] */
        Dim ItemList() As Byte      '/* Array of ItemType values starts here       */
    End Structure

    '/* DAT_EVENT. For passing events down from the application to the DS. */
    <StructLayout(LayoutKind.Sequential, Pack:=2, CharSet:=CharSet.Ansi)> _
    Public Structure TW_EVENT
        Dim pEvent As IntPtr      '/* Windows pMSG or Mac pEvent.                 */
        Dim TWMessage As MSG      '/* TW msg from data source, e.g. MSG_XFERREADY */
    End Structure

    'typedef struct {
    '    TW_UINT16   InfoID;
    '    TW_UINT16   ItemType;
    '    TW_UINT16   NumItems;
    '    union {
    '      TW_UINT16   CondCode;    /* Depreciated, use ReturnCode. TWAIN 2.0 and older. */
    '      TW_UINT16   ReturnCode;  /* TWAIN 2.1 and newer */
    '    };
    '    TW_UINTPTR  Item;
    '}TW_INFO, FAR* pTW_INFO;

    'typedef struct {
    '    TW_UINT32   NumInfos;
    '    TW_INFO     Info[1];
    '}TW_EXTIMAGEINFO, FAR* pTW_EXTIMAGEINFO;

    '/* DAT_FILESYSTEM, information about TWAIN file system */
    'typedef struct {
    '   /* DG_CONTROL / DAT_FILESYSTEM / MSG_xxxx fields     */
    '   TW_STR255  InputName; /* The name of the input or source file */
    '   TW_STR255  OutputName; /* The result of an operation or the name of a destination file */
    '   TW_MEMREF  Context; /* Source specific data used to remember state information */
    '   /* DG_CONTROL / DAT_FILESYSTEM / MSG_DELETE field    */
    '   int        Recursive; /* recursively delete all sub-directories */
    '   /* DG_CONTROL / DAT_FILESYSTEM / MSG_GETINFO fields  */
    '   TW_INT32   FileType; /* One of the TWFY_xxxx values */
    '   TW_UINT32  Size; /* Size of current FileType */
    '   TW_STR32   CreateTimeDate; /* creation date of the file */
    '   TW_STR32   ModifiedTimeDate; /* last date the file was modified */
    '   TW_UINT32  FreeSpace; /* bytes of free space on the current device */
    '   TW_INT32   NewImageSize; /* estimate of the amount of space a new image would take up */
    '   TW_UINT32  NumberOfFiles; /* number of files, depends on FileType */
    '   TW_UINT32  NumberOfSnippets; /* number of audio snippets */
    '   TW_UINT32  DeviceGroupMask; /* used to group cameras (ex: front/rear bitonal, front/rear grayscale...) */
    '   char       Reserved[508]; /**/
    '} TW_FILESYSTEM, FAR * pTW_FILESYSTEM;

    '/* DAT_GRAYRESPONSE */
    'typedef struct {
    '   TW_ELEMENT8         Response[1];
    '} TW_GRAYRESPONSE, FAR * pTW_GRAYRESPONSE;

    '/* No DAT needed.  Describes version of software currently running. */
    'typedef struct {
    '   TW_UINT16  MajorNum;  /* Major revision number of the software. */
    '   TW_UINT16  MinorNum;  /* Incremental revision number of the software. */
    '   TW_UINT16  Language;  /* e.g. TWLG_SWISSFRENCH */
    '   TW_UINT16  Country;   /* e.g. TWCY_SWITZERLAND */
    '   TW_STR32   Info;      /* e.g. "1.0b3 Beta release" */
    '} TW_VERSION, FAR * pTW_VERSION;

    '/* DAT_IDENTITY. Identifies the program/library/code resource. */
    'typedef struct {
    '   TW_UINT32  Id;              /* Unique number.  In Windows, application hWnd      */
    '   TW_VERSION Version;         /* Identifies the piece of code              */
    '   TW_UINT16  ProtocolMajor;   /* Application and DS must set to TWON_PROTOCOLMAJOR */
    '   TW_UINT16  ProtocolMinor;   /* Application and DS must set to TWON_PROTOCOLMINOR */
    '   TW_UINT32  SupportedGroups; /* Bit field OR combination of DG_ constants */
    '   TW_STR32   Manufacturer;    /* Manufacturer name, e.g. "Hewlett-Packard" */
    '   TW_STR32   ProductFamily;   /* Product family name, e.g. "ScanJet"       */
    '   TW_STR32   ProductName;     /* Product name, e.g. "ScanJet Plus"         */
    '} TW_IDENTITY, FAR * pTW_IDENTITY;

    '/* DAT_IMAGEINFO. Application gets detailed image info from DS with this. */
    <StructLayout(LayoutKind.Sequential, Pack:=2, CharSet:=CharSet.Ansi)> _
    Public Structure TW_IMAGEINFO
        Dim XResolution As TW_FIX32  '/* Resolution in the horizontal             */
        Dim YResolution As TW_FIX32  '/* Resolution in the vertical               */
        Dim ImageWidth As Int32      '/* Columns in the image, -1 if unknown by DS*/
        Dim ImageLength As Int32     '/* Rows in the image, -1 if unknown by DS   */
        Dim SamplesPerPixel As Int16 '/* Number of samples per pixel, 3 for RGB   */
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=8)> _
        Dim BitsPerSample() As Int16 '/* Number of bits for each sample           */
        Dim BitsPerPixel As Int16   '/* Number of bits for each padded pixel     */
        Dim Planar As UInt16        '/* True if Planar, False if chunky          */
        Dim PixelType As TWPT       '/* How to interp data; photo interp (TWPT_) */
        Dim Compression As TWCP    '/* How the data is compressed (TWCP_xxxx)   */
    End Structure

    '/* DAT_IMAGELAYOUT. Provides image layout information in current units. */
    <StructLayout(LayoutKind.Sequential, Pack:=2, CharSet:=CharSet.Ansi)> _
    Public Structure TW_IMAGELAYOUT
        Dim Frame As TW_FRAME        '/* Frame coords within larger document */
        Dim DocumentNumber As UInt32
        Dim PageNumber As UInt32     '/* Reset when you go to next document  */
        Dim FrameNumber As UInt32    '/* Reset when you go to next page      */
    End Structure

    '/* No DAT needed.  Used to manage memory buffers. */
    <StructLayout(LayoutKind.Sequential, Pack:=2, CharSet:=CharSet.Ansi)> _
    Public Structure TW_MEMORY
        Dim Flags As UInt32  '/* Any combination of the TWMF_ constants.           */
        Dim Length As UInt32 '/* Number of bytes stored in buffer TheMem.          */
        Dim TheMem As IntPtr '/* Pointer or handle to the allocated memory buffer. */
    End Structure

    '/* DAT_IMAGEMEMXFER. Used to pass image data (e.g. in strips) from DS to application.*/
    <StructLayout(LayoutKind.Sequential, Pack:=2, CharSet:=CharSet.Ansi)> _
    Public Structure TW_IMAGEMEMXFER
        Dim Compression As UInt16  '/* How the data is compressed                */
        Dim BytesPerRow As UInt32  '/* Number of bytes in a row of data          */
        Dim Columns As UInt32      '/* How many columns                          */
        Dim Rows As UInt32         '/* How many rows                             */
        Dim XOffset As UInt32      '/* How far from the side of the image        */
        Dim YOffset As UInt32      '/* How far from the top of the image         */
        Dim BytesWritten As UInt32 '/* How many bytes written in Memory          */
        Dim Memory As TW_MEMORY    '/* Mem struct used to pass actual image data */
    End Structure

    '/* Changed in 1.1: QuantTable, HuffmanDC, HuffmanAC TW_MEMREF -> TW_MEMORY  */
    '/* DAT_JPEGCOMPRESSION. Based on JPEG Draft International Std, ver 10918-1. */
    'typedef struct {
    '   TW_UINT16   ColorSpace;       /* One of the TWPT_xxxx values                */
    '   TW_UINT32   SubSampling;      /* Two word "array" for subsampling values    */
    '   TW_UINT16   NumComponents;    /* Number of color components in image        */
    '   TW_UINT16   RestartFrequency; /* Frequency of restart marker codes in MDU's */
    '   TW_UINT16   QuantMap[4];      /* Mapping of components to QuantTables       */
    '   TW_MEMORY   QuantTable[4];    /* Quantization tables                        */
    '   TW_UINT16   HuffmanMap[4];    /* Mapping of components to Huffman tables    */
    '   TW_MEMORY   HuffmanDC[2];     /* DC Huffman tables                          */
    '   TW_MEMORY   HuffmanAC[2];     /* AC Huffman tables                          */
    '} TW_JPEGCOMPRESSION, FAR * pTW_JPEGCOMPRESSION;

    '/* TWON_ONEVALUE. Container for one value. */
    <StructLayout(LayoutKind.Sequential, Pack:=2, CharSet:=CharSet.Ansi)> _
    Public Structure TW_ONEVALUE
        Dim ItemType As UInt16
        Dim Item As Int32   'XXX the spec says UInt32 here, but then they assign negative values to it!
    End Structure
    <StructLayout(LayoutKind.Sequential, Pack:=2, CharSet:=CharSet.Ansi)> _
    Public Structure TW_ONEVALUE_FIX32
        Dim ItemType As UInt16
        Dim Item As TW_FIX32
    End Structure
    '<StructLayout(LayoutKind.Sequential, Pack:=2, CharSet:=CharSet.Ansi)> _
    'Public Structure TW_ONEVALUE_INT16
    '    Dim ItemType As UInt16
    '    Dim Item As Int16
    'End Structure


    '/* DAT_PALETTE8. Color palette when TWPT_PALETTE pixels xfer'd in mem buf. */
    'typedef struct {
    '   TW_UINT16    NumColors;   /* Number of colors in the color table.  */
    '   TW_UINT16    PaletteType; /* TWPA_xxxx, specifies type of palette. */
    '   TW_ELEMENT8  Colors[256]; /* Array of palette values starts here.  */
    '} TW_PALETTE8, FAR * pTW_PALETTE8;

    '/* DAT_PASSTHRU, device dependant data to pass through Data Source */
    'typedef struct {
    '   TW_MEMREF  pCommand;        /* Pointer to Command buffer */
    '   TW_UINT32  CommandBytes;    /* Number of bytes in Command buffer */
    '   TW_INT32   Direction;       /* One of the TWDR_xxxx values.  Defines the direction of data flow */
    '   TW_MEMREF  pData;           /* Pointer to Data buffer */
    '   TW_UINT32  DataBytes;       /* Number of bytes in Data buffer */
    '   TW_UINT32  DataBytesXfered; /* Number of bytes successfully transferred */
    '} TW_PASSTHRU, FAR * pTW_PASSTHRU;

    '/* DAT_PENDINGXFERS. Used with MSG_ENDXFER to indicate additional data. */
    <StructLayout(LayoutKind.Sequential, Pack:=2, CharSet:=CharSet.Ansi)> _
    Public Structure TW_PENDINGXFERS
        'Dim Count As UInt16 'XXX another instance of the TWAIN standard specifying a UINT with a negative value
        Dim Count As Int16
        Dim EOJ As UInt32
        '   union {
        '      TW_UINT32 EOJ;
        '      TW_UINT32 Reserved;
        '   };
    End Structure

    '/* TWON_RANGE. Container for a range of values. */
    <StructLayout(LayoutKind.Sequential, Pack:=2, CharSet:=CharSet.Ansi)> _
    Public Structure TW_RANGE
        Dim ItemType As UInt16
        Dim MinValue As UInt32    '/* Starting value in the range.           */
        Dim MaxValue As UInt32    '/* Final value in the range.              */
        Dim StepSize As UInt32   '/* Increment from MinValue to MaxValue.   */
        Dim DefaultValue As UInt32 '/* Power-up value.                        */
        Dim CurrentValue As UInt32 '/* The value that is currently in effect. */
    End Structure

    '/* DAT_RGBRESPONSE */
    'typedef struct {
    '   TW_ELEMENT8         Response[1];
    '} TW_RGBRESPONSE, FAR * pTW_RGBRESPONSE;

    '/* DAT_SETUPFILEXFER. Sets up DS to application data transfer via a file. */
    'typedef struct {
    '   TW_STR255 FileName;
    '   TW_UINT16 Format;   /* Any TWFF_ constant */
    '   TW_INT16  VRefNum;  /* Used for Mac only  */
    '} TW_SETUPFILEXFER, FAR * pTW_SETUPFILEXFER;

    '/* DAT_SETUPMEMXFER. Sets up DS to application data transfer via a memory buffer. */
    <StructLayout(LayoutKind.Sequential, Pack:=2, CharSet:=CharSet.Ansi)> _
    Public Structure TW_SETUPMEMXFER
        Dim MinBufSize As UInt32
        Dim MaxBufSize As UInt32
        Dim Preferred As UInt32
    End Structure

    '/* DAT_STATUS. Application gets detailed status info from a data source with this. */
    <StructLayout(LayoutKind.Sequential, Pack:=2, CharSet:=CharSet.Ansi)> _
    Public Structure TW_STATUS
        Dim ConditionCode As UInt16  '/* Any TWCC_ constant     */
        Dim Data As UInt16
    End Structure

    '/* DAT_STATUSUTF8. Application gets detailed UTF8 status info from a data source with this.  Added 2.1 */
    'typedef struct {
    '   TW_STATUS    Status;         /* input  TW_STATUS data received from a previous call to DG_CONTROL / DAT_STATUS / MSG_GET. */
    '   TW_UINT32    Size;           /* output Total number of bytes in the UTF8string, plus the terminating NUL byte.  This is not the same as the total number of characters in the string. */
    '   TW_HANDLE    UTF8string;     /* output TW_HANDLE to a UTF-8 encoded localized string (based on TW_IDENTITY.Language or CAP_LANGUAGE).  The Source allocates it, the Application frees it. */
    '} TW_STATUSUTF8, FAR * pTW_STATUSUTF8;

    '/* DAT_USERINTERFACE. Coordinates UI between application and data source. */
    <StructLayout(LayoutKind.Sequential, Pack:=2, CharSet:=CharSet.Ansi)> _
    Public Structure TW_USERINTERFACE
        Dim ShowUI As UInt16  '/* TRUE if DS should bring up its UI           */
        Dim ModalUI As UInt16 '/* For Mac only - true if the DS's UI is modal */
        Dim hParent As IntPtr '/* For windows only - Application window handle        */
    End Structure


    '/****************************************************************************
    ' * Generic Constants                                                        *
    ' ****************************************************************************/
    Public Enum TWON As UInt32
        TWON_ARRAY = 3 '/* indicates TW_ARRAY container       */
        TWON_ENUMERATION = 4 '/* indicates TW_ENUMERATION container */
        TWON_ONEVALUE = 5 '/* indicates TW_ONEVALUE container    */
        TWON_RANGE = 6 '/* indicates TW_RANGE container       */

        TWON_ICONID = 962 '/* res Id of icon used in USERSELECT lbox */
        TWON_DSMID = 461 '/* res Id of the DSM version num resource */
        TWON_DSMCODEID = 63  '/* res Id of the Mac SM Code resource     */

        TWON_DONTCARE8 = &HFF
        TWON_DONTCARE16 = &HFFFF
        TWON_DONTCARE32 = &HFFFFFFFFUI
    End Enum

    ''/* Flags used in TW_MEMORY structure. */
    Public Enum TWMF As UInt16
        TWMF_APPOWNS = &H1
        TWMF_DSMOWNS = &H2
        TWMF_DSOWNS = &H4
        TWMF_POINTER = &H8
        TWMF_HANDLE = &H10
    End Enum
    Public Enum TWTY As UInt16
        ''/* There are four containers used for capabilities negotiation:
        '' *    TWON_ONEVALUE, TWON_RANGE, TWON_ENUMERATION, TWON_ARRAY
        '' * In each container structure ItemType can be TWTY_INT8, TWTY_INT16, etc.
        '' * The kind of data stored in the container can be determined by doing
        '' * DCItemSize[ItemType] where the following is defined in TWAIN glue code:
        '' *          DCItemSize[]= { sizeof(TW_INT8),
        '' *                          sizeof(TW_INT16),
        '' *                          etc.
        '' *                          sizeof(TW_UINT32) };
        '' *
        '' */

        TWTY_INT8 = &H0    '/* Means Item is a TW_INT8   */
        TWTY_INT16 = &H1    '/* Means Item is a TW_INT16  */
        TWTY_INT32 = &H2    '/* Means Item is a TW_INT32  */

        TWTY_UINT8 = &H3    '/* Means Item is a TW_UINT8  */
        TWTY_UINT16 = &H4    '/* Means Item is a TW_UINT16 */
        TWTY_UINT32 = &H5    '/* Means Item is a TW_UINT32 */

        TWTY_BOOL = &H6    '/* Means Item is a TW_BOOL   */

        TWTY_FIX32 = &H7    '/* Means Item is a TW_FIX32  */

        TWTY_FRAME = &H8    '/* Means Item is a TW_FRAME  */

        TWTY_STR32 = &H9    '/* Means Item is a TW_STR32  */
        TWTY_STR64 = &HA    '/* Means Item is a TW_STR64  */
        TWTY_STR128 = &HB    '/* Means Item is a TW_STR128 */
        TWTY_STR255 = &HC    '/* Means Item is a TW_STR255 */
    End Enum
    '/****************************************************************************
    ' * Capability Constants                                                     *
    ' ****************************************************************************/

    '/* CAP_ALARMS values (AL_ means alarms) Added 1.8  */
    '#define TWAL_ALARM               0
    '#define TWAL_FEEDERERROR         1
    '#define TWAL_FEEDERWARNING       2
    '#define TWAL_BARCODE             3
    '#define TWAL_DOUBLEFEED          4
    '#define TWAL_JAM                 5
    '#define TWAL_PATCHCODE           6
    '#define TWAL_POWER               7
    '#define TWAL_SKEW                8

    '/* ICAP_AUTOSIZE values Added 2.0 */
    '#define TWAS_NONE                0
    '#define TWAS_AUTO                1
    '#define TWAS_CURRENT             2

    '/* TWEI_BARCODEROTATION values (BCOR_ means barcode rotation) Added 1.7 */
    '#define TWBCOR_ROT0              0
    '#define TWBCOR_ROT90             1
    '#define TWBCOR_ROT180            2
    '#define TWBCOR_ROT270            3
    '#define TWBCOR_ROTX              4

    '/* ICAP_BARCODESEARCHMODE values (TWBD_ means search) */
    '#define TWBD_HORZ                0
    '#define TWBD_VERT                1
    '#define TWBD_HORZVERT            2
    '#define TWBD_VERTHORZ            3

    '/* ICAP_BITORDER values (BO_ means Bit Order) */
    Public Enum TWBO As UInt16
        TWBO_LSBFIRST = 0
        TWBO_MSBFIRST = 1
    End Enum

    '/* ICAP_AUTODISCARDBLANKPAGES values Added 2.0 */
    '#define TWBP_DISABLE            -2
    '#define TWBP_AUTO               -1

    '/* ICAP_BITDEPTHREDUCTION values (BR_ means Bitdepth Reduction) Added 1.5 */
    '#define TWBR_THRESHOLD           0
    '#define TWBR_HALFTONE            1
    '#define TWBR_CUSTHALFTONE        2
    '#define TWBR_DIFFUSION           3

    '/* ICAP_SUPPORTEDBARCODETYPES and TWEI_BARCODETYPE values Added 1.7 */
    '#define TWBT_3OF9                 0
    '#define TWBT_2OF5INTERLEAVED      1
    '#define TWBT_2OF5NONINTERLEAVED   2
    '#define TWBT_CODE93               3
    '#define TWBT_CODE128              4
    '#define TWBT_UCC128               5
    '#define TWBT_CODABAR              6
    '#define TWBT_UPCA                 7
    '#define TWBT_UPCE                 8
    '#define TWBT_EAN8                 9
    '#define TWBT_EAN13                10
    '#define TWBT_POSTNET              11
    '#define TWBT_PDF417               12
    '#define TWBT_2OF5INDUSTRIAL       13 /* Added 1.8 */
    '#define TWBT_2OF5MATRIX           14 /* Added 1.8 */
    '#define TWBT_2OF5DATALOGIC        15 /* Added 1.8 */
    '#define TWBT_2OF5IATA             16 /* Added 1.8 */
    '#define TWBT_3OF9FULLASCII        17 /* Added 1.8 */
    '#define TWBT_CODABARWITHSTARTSTOP 18 /* Added 1.8 */
    '#define TWBT_MAXICODE             19 /* Added 1.8 */

    '/* ICAP_COMPRESSION values (CP_ means ComPression ) */
    Public Enum TWCP As UInt16
        TWCP_NONE = 0
        TWCP_PACKBITS = 1
        TWCP_GROUP31D = 2 '/* Follows CCITT spec (no End Of Line)          */
        TWCP_GROUP31DEOL = 3 '/* Follows CCITT spec (has End Of Line)         */
        TWCP_GROUP32D = 4 '/* Follows CCITT spec (use cap for K Factor)    */
        TWCP_GROUP4 = 5 '/* Follows CCITT spec                           */
        TWCP_JPEG = 6 '/* Use capability for more info                 */
        TWCP_LZW = 7 '/* Must license from Unisys and IBM to use      */
        TWCP_JBIG = 8 '/* For Bitonal images  -- Added 1.7 KHL         */
        TWCP_PNG = 9 '/* Added 1.8 */
        TWCP_RLE4 = 10 '/* Added 1.8 */
        TWCP_RLE8 = 11 '/* Added 1.8 */
        TWCP_BITFIELDS = 12 '/* Added 1.8 */
    End Enum
    '/* CAP_CAMERASIDE and TWEI_PAGESIDE values (CS_ means camera side) Added 1.91 */
    '#define TWCS_BOTH                0
    '#define TWCS_TOP                 1
    '#define TWCS_BOTTOM              2

    '/* CAP_CLEARBUFFERS values (CB_ means clear buffers) */
    '#define TWCB_AUTO                0
    '#define TWCB_CLEAR               1
    '#define TWCB_NOCLEAR             2

    '/* CAP_DEVICEEVENT values (DE_ means device event) */
    '#define TWDE_CUSTOMEVENTS           0x8000      
    '#define TWDE_CHECKAUTOMATICCAPTURE  0
    '#define TWDE_CHECKBATTERY           1
    '#define TWDE_CHECKDEVICEONLINE      2
    '#define TWDE_CHECKFLASH             3
    '#define TWDE_CHECKPOWERSUPPLY       4
    '#define TWDE_CHECKRESOLUTION        5
    '#define TWDE_DEVICEADDED            6
    '#define TWDE_DEVICEOFFLINE          7
    '#define TWDE_DEVICEREADY            8
    '#define TWDE_DEVICEREMOVED          9
    '#define TWDE_IMAGECAPTURED          10
    '#define TWDE_IMAGEDELETED           11
    '#define TWDE_PAPERDOUBLEFEED        12
    '#define TWDE_PAPERJAM               13
    '#define TWDE_LAMPFAILURE            14
    '#define TWDE_POWERSAVE              15
    '#define TWDE_POWERSAVENOTIFY        16

    '/* TW_PASSTHRU.Direction values.  Added 1.8 */
    '#define TWDR_GET                 1
    '#define TWDR_SET                 2   

    '/* TWEI_DESKEWSTATUS values Added 1.7 */
    '#define TWDSK_SUCCESS            0
    '#define TWDSK_REPORTONLY         1
    '#define TWDSK_FAIL               2
    '#define TWDSK_DISABLED           3

    '/* CAP_DUPLEX values Added 1.7 */
    Public Enum TWDX As UInt16
        TWDX_NONE = 0
        TWDX_1PASSDUPLEX = 1
        TWDX_2PASSDUPLEX = 2
    End Enum
    '/* CAP_FEEDERALIGNMENT values (FA_ means feeder alignment) */
    '#define TWFA_NONE                0
    '#define TWFA_LEFT                1
    '#define TWFA_CENTER              2
    '#define TWFA_RIGHT               3

    '/* ICAP_FEEDERTYPE */
    '#define TWFE_GENERAL             0
    '#define TWFE_PHOTO               1

    '/* ICAP_IMAGEFILEFORMAT values (FF_means File Format)   */
    '#define TWFF_TIFF                0    /* Tagged Image File Format     */
    '#define TWFF_PICT                1    /* Macintosh PICT               */
    '#define TWFF_BMP                 2    /* Windows Bitmap               */
    '#define TWFF_XBM                 3    /* X-Windows Bitmap             */
    '#define TWFF_JFIF                4    /* JPEG File Interchange Format */
    '#define TWFF_FPX                 5    /* Flash Pix                    */
    '#define TWFF_TIFFMULTI           6    /* Multi-page tiff file         */
    '#define TWFF_PNG                 7
    '#define TWFF_SPIFF               8
    '#define TWFF_EXIF                9
    '#define TWFF_PDF                10    /* 1.91 NB: this is not PDF/A */
    '#define TWFF_JP2                11    /* 1.91 */
    '#define TWFF_JPX                13    /* 1.91 */
    '#define TWFF_DEJAVU             14    /* 1.91 */
    '#define TWFF_PDFA               15    /* 2.0 Adobe PDF/A, Version 1*/
    '#define TWFF_PDFA2              16    /* 2.1 Adobe PDF/A, Version 2*/

    '/* ICAP_FLASHUSED2 values (FL_ means flash) */
    '#define TWFL_NONE                0
    '#define TWFL_OFF                 1
    '#define TWFL_ON                  2
    '#define TWFL_AUTO                3
    '#define TWFL_REDEYE              4

    '/* CAP_FEEDERORDER values (FO_ means feeder order) */
    '#define TWFO_FIRSTPAGEFIRST      0
    '#define TWFO_LASTPAGEFIRST       1

    '/* CAP_FEEDERPOCKET */
    '#define TWFP_POCKETERROR         0
    '#define TWFP_POCKET1             1
    '#define TWFP_POCKET2             2
    '#define TWFP_POCKET3             3
    '#define TWFP_POCKET4             4
    '#define TWFP_POCKET5             5
    '#define TWFP_POCKET6             6
    '#define TWFP_POCKET7             7
    '#define TWFP_POCKET8             8
    '#define TWFP_POCKET9             9
    '#define TWFP_POCKET10           10
    '#define TWFP_POCKET11           11
    '#define TWFP_POCKET12           12
    '#define TWFP_POCKET13           13
    '#define TWFP_POCKET14           14
    '#define TWFP_POCKET15           15
    '#define TWFP_POCKET16           16

    '/* ICAP_FLIPROTATION values (FR_ means flip rotation) */
    '#define TWFR_BOOK                0
    '#define TWFR_FANFOLD             1

    '/* ICAP_FILTER values (FT_ means Filter Type) */
    '#define TWFT_RED                 0
    '#define TWFT_GREEN               1
    '#define TWFT_BLUE                2
    '#define TWFT_NONE                3
    '#define TWFT_WHITE               4
    '#define TWFT_CYAN                5
    '#define TWFT_MAGENTA             6
    '#define TWFT_YELLOW              7
    '#define TWFT_BLACK               8

    '/* TW_FILESYSTEM.FileType values (FY_ means file type) */
    '#define TWFY_CAMERA              0
    '#define TWFY_CAMERATOP           1
    '#define TWFY_CAMERABOTTOM        2
    '#define TWFY_CAMERAPREVIEW       3
    '#define TWFY_DOMAIN              4
    '#define TWFY_HOST                5
    '#define TWFY_DIRECTORY           6
    '#define TWFY_IMAGE               7
    '#define TWFY_UNKNOWN             8

    '/* ICAP_ICCPROFILE */ 
    '#define TWIC_NONE                0
    '#define TWIC_LINK                1
    '#define TWIC_EMBED               2

    '/* ICAP_IMAGEFILTER values (IF_ means image filter) */
    '#define TWIF_NONE                0
    '#define TWIF_AUTO                1
    '#define TWIF_LOWPASS             2
    '#define TWIF_BANDPASS            3
    '#define TWIF_HIGHPASS            4
    '#define TWIF_TEXT                TWIF_BANDPASS
    '#define TWIF_FINELINE            TWIF_HIGHPASS

    '/* ICAP_IMAGEMERGE values (IM_ means image merge) */
    '#define TWIM_NONE                0
    '#define TWIM_FRONTONTOP          1
    '#define TWIM_FRONTONBOTTOM       2
    '#define TWIM_FRONTONLEFT         3
    '#define TWIM_FRONTONRIGHT        4

    '/* CAP_JOBCONTROL values Added 1.7  */
    '#define TWJC_NONE                0
    '#define TWJC_JSIC                1
    '#define TWJC_JSIS                2
    '#define TWJC_JSXC                3
    '#define TWJC_JSXS                4

    '/* ICAP_JPEGQUALITY values (JQ_ means jpeg quality) */
    '#define TWJQ_UNKNOWN            -4 
    '#define TWJQ_LOW                -3
    '#define TWJQ_MEDIUM             -2
    '#define TWJQ_HIGH               -1

    '/* ICAP_LIGHTPATH values (LP_ means Light Path) */
    '#define TWLP_REFLECTIVE          0
    '#define TWLP_TRANSMISSIVE        1

    '/* ICAP_LIGHTSOURCE values (LS_ means Light Source) */
    '#define TWLS_RED                 0
    '#define TWLS_GREEN               1
    '#define TWLS_BLUE                2
    '#define TWLS_NONE                3
    '#define TWLS_WHITE               4
    '#define TWLS_UV                  5
    '#define TWLS_IR                  6

    '/* TWEI_MAGTYPE values (MD_ means Mag Type) Added 2.0 */
    '#define TWMD_MICR                0  /* Added 2.0 */
    '#define TWMD_RAW                 1  /* added 2.1 */
    '#define TWMD_INVALID             2  /* added 2.1 */

    '/* ICAP_NOISEFILTER values (NF_ means noise filter) */
    '#define TWNF_NONE                0
    '#define TWNF_AUTO                1
    '#define TWNF_LONEPIXEL           2
    '#define TWNF_MAJORITYRULE        3

    '/* ICAP_ORIENTATION values (OR_ means ORientation) */
    '#define TWOR_ROT0                0
    '#define TWOR_ROT90               1
    '#define TWOR_ROT180              2
    '#define TWOR_ROT270              3
    '#define TWOR_PORTRAIT            TWOR_ROT0
    '#define TWOR_LANDSCAPE           TWOR_ROT270
    '#define TWOR_AUTO                4           /* 2.0 */
    '#define TWOR_AUTOTEXT            5           /* 2.0 */
    '#define TWOR_AUTOPICTURE         6           /* 2.0 */

    '/* ICAP_OVERSCAN values (OV_ means overscan) */
    '#define TWOV_NONE                0
    '#define TWOV_AUTO                1
    '#define TWOV_TOPBOTTOM           2
    '#define TWOV_LEFTRIGHT           3
    '#define TWOV_ALL                 4

    '/* Palette types for TW_PALETTE8 */
    '#define TWPA_RGB         0
    '#define TWPA_GRAY        1
    '#define TWPA_CMY         2

    '/* ICAP_PLANARCHUNKY values (PC_ means Planar/Chunky ) */
    Public Enum TWPC As UInt16
        TWPC_CHUNKY = 0
        TWPC_PLANAR = 1
    End Enum
    '/* TWEI_PATCHCODE values Added 1.7 */
    '#define TWPCH_PATCH1             0
    '#define TWPCH_PATCH2             1
    '#define TWPCH_PATCH3             2
    '#define TWPCH_PATCH4             3
    '#define TWPCH_PATCH6             4
    '#define TWPCH_PATCHT             5

    '/* ICAP_PIXELFLAVOR values (PF_ means Pixel Flavor) */
    Public Enum TWPF As UInt16
        TWPF_CHOCOLATE = 0  '/* zero pixel represents darkest shade  */
        TWPF_VANILLA = 1  '/* zero pixel represents lightest shade */
    End Enum
    '/* CAP_PRINTERMODE values (PM_ means printer mode) */
    '#define TWPM_SINGLESTRING        0
    '#define TWPM_MULTISTRING         1
    '#define TWPM_COMPOUNDSTRING      2

    '/* CAP_PRINTER values (PR_ means printer) */
    '#define TWPR_IMPRINTERTOPBEFORE     0
    '#define TWPR_IMPRINTERTOPAFTER      1
    '#define TWPR_IMPRINTERBOTTOMBEFORE  2
    '#define TWPR_IMPRINTERBOTTOMAFTER   3
    '#define TWPR_ENDORSERTOPBEFORE      4
    '#define TWPR_ENDORSERTOPAFTER       5
    '#define TWPR_ENDORSERBOTTOMBEFORE   6
    '#define TWPR_ENDORSERBOTTOMAFTER    7

    '/* CAP_POWERSUPPLY values (PS_ means power supply) */
    '#define TWPS_EXTERNAL            0
    '#define TWPS_BATTERY             1

    '/* ICAP_PIXELTYPE values (PT_ means Pixel Type) */
    Public Enum TWPT As UInt16
        TWPT_BW = 0 '/* Black and White */
        TWPT_GRAY = 1
        TWPT_RGB = 2
        TWPT_PALETTE = 3
        TWPT_CMY = 4
        TWPT_CMYK = 5
        TWPT_YUV = 6
        TWPT_YUVK = 7
        TWPT_CIEXYZ = 8
        TWPT_LAB = 9
        TWPT_SRGB = 10 '/* 1.91 */
        TWPT_SCRGB = 11 '/* 1.91 */
        TWPT_INFRARED = 16 '/* 2.0 */
    End Enum

    '/* CAP_SEGMENTED values (SG_ means segmented) Added 1.91 */
    '#define TWSG_NONE                0
    '#define TWSG_AUTO                1

    '/* ICAP_SUPPORTEDSIZES values (SS_ means Supported Sizes) */
    '#define TWSS_NONE                0
    '#define TWSS_A4                  1
    '#define TWSS_JISB5               2
    '#define TWSS_USLETTER            3
    '#define TWSS_USLEGAL             4
    '/* Added 1.5 */
    '#define TWSS_A5                  5
    '#define TWSS_ISOB4               6
    '#define TWSS_ISOB6               7
    '/* Added 1.7 */
    '#define TWSS_USLEDGER            9
    '#define TWSS_USEXECUTIVE        10
    '#define TWSS_A3                 11
    '#define TWSS_ISOB3              12
    '#define TWSS_A6                 13
    '#define TWSS_C4                 14
    '#define TWSS_C5                 15
    '#define TWSS_C6                 16
    '/* Added 1.8 */
    '#define TWSS_4A0                17
    '#define TWSS_2A0                18
    '#define TWSS_A0                 19
    '#define TWSS_A1                 20
    '#define TWSS_A2                 21
    '#define TWSS_A7                 22
    '#define TWSS_A8                 23
    '#define TWSS_A9                 24
    '#define TWSS_A10                25
    '#define TWSS_ISOB0              26
    '#define TWSS_ISOB1              27
    '#define TWSS_ISOB2              28
    '#define TWSS_ISOB5              29
    '#define TWSS_ISOB7              30
    '#define TWSS_ISOB8              31
    '#define TWSS_ISOB9              32
    '#define TWSS_ISOB10             33
    '#define TWSS_JISB0              34
    '#define TWSS_JISB1              35
    '#define TWSS_JISB2              36
    '#define TWSS_JISB3              37
    '#define TWSS_JISB4              38
    '#define TWSS_JISB6              39
    '#define TWSS_JISB7              40
    '#define TWSS_JISB8              41
    '#define TWSS_JISB9              42
    '#define TWSS_JISB10             43
    '#define TWSS_C0                 44
    '#define TWSS_C1                 45
    '#define TWSS_C2                 46
    '#define TWSS_C3                 47
    '#define TWSS_C7                 48
    '#define TWSS_C8                 49
    '#define TWSS_C9                 50
    '#define TWSS_C10                51
    '#define TWSS_USSTATEMENT        52
    '#define TWSS_BUSINESSCARD       53
    '#define TWSS_MAXSIZE            54  /* Added 2.1 */

    '/* ICAP_XFERMECH values (SX_ means Setup XFer) */
    Public Enum TWSX As UInt16
        TWSX_NATIVE = 0
        TWSX_FILE = 1
        TWSX_MEMORY = 2
        TWSX_MEMFILE = 4    '/* added 1.91 */
    End Enum

    '/* ICAP_UNITS values (UN_ means UNits) */
    Public Enum TWUN As UInt16
        TWUN_INCHES = 0
        TWUN_CENTIMETERS = 1
        TWUN_PICAS = 2
        TWUN_POINTS = 3
        TWUN_TWIPS = 4
        TWUN_PIXELS = 5
        TWUN_MILLIMETERS = 6    '/* added 1.91 */
    End Enum

    '/****************************************************************************
    ' * Country Constants                                                        *
    ' ****************************************************************************/
    Public Enum TWCY As UInt16
        TWCY_AFGHANISTAN = 1001
        TWCY_ALGERIA = 213
        TWCY_AMERICANSAMOA = 684
        TWCY_ANDORRA = 33
        TWCY_ANGOLA = 1002
        TWCY_ANGUILLA = 8090
        TWCY_ANTIGUA = 8091
        TWCY_ARGENTINA = 54
        TWCY_ARUBA = 297
        TWCY_ASCENSIONI = 247
        TWCY_AUSTRALIA = 61
        TWCY_AUSTRIA = 43
        TWCY_BAHAMAS = 8092
        TWCY_BAHRAIN = 973
        TWCY_BANGLADESH = 880
        TWCY_BARBADOS = 8093
        TWCY_BELGIUM = 32
        TWCY_BELIZE = 501
        TWCY_BENIN = 229
        TWCY_BERMUDA = 8094
        TWCY_BHUTAN = 1003
        TWCY_BOLIVIA = 591
        TWCY_BOTSWANA = 267
        TWCY_BRITAIN = 6
        TWCY_BRITVIRGINIS = 8095
        TWCY_BRAZIL = 55
        TWCY_BRUNEI = 673
        TWCY_BULGARIA = 359
        TWCY_BURKINAFASO = 1004
        TWCY_BURMA = 1005
        TWCY_BURUNDI = 1006
        TWCY_CAMAROON = 237
        TWCY_CANADA = 2
        TWCY_CAPEVERDEIS = 238
        TWCY_CAYMANIS = 8096
        TWCY_CENTRALAFREP = 1007
        TWCY_CHAD = 1008
        TWCY_CHILE = 56
        TWCY_CHINA = 86
        TWCY_CHRISTMASIS = 1009
        TWCY_COCOSIS = 1009
        TWCY_COLOMBIA = 57
        TWCY_COMOROS = 1010
        TWCY_CONGO = 1011
        TWCY_COOKIS = 1012
        TWCY_COSTARICA = 506
        TWCY_CUBA = 5
        TWCY_CYPRUS = 357
        TWCY_CZECHOSLOVAKIA = 42
        TWCY_DENMARK = 45
        TWCY_DJIBOUTI = 1013
        TWCY_DOMINICA = 8097
        TWCY_DOMINCANREP = 8098
        TWCY_EASTERIS = 1014
        TWCY_ECUADOR = 593
        TWCY_EGYPT = 20
        TWCY_ELSALVADOR = 503
        TWCY_EQGUINEA = 1015
        TWCY_ETHIOPIA = 251
        TWCY_FALKLANDIS = 1016
        TWCY_FAEROEIS = 298
        TWCY_FIJIISLANDS = 679
        TWCY_FINLAND = 358
        TWCY_FRANCE = 33
        TWCY_FRANTILLES = 596
        TWCY_FRGUIANA = 594
        TWCY_FRPOLYNEISA = 689
        TWCY_FUTANAIS = 1043
        TWCY_GABON = 241
        TWCY_GAMBIA = 220
        TWCY_GERMANY = 49
        TWCY_GHANA = 233
        TWCY_GIBRALTER = 350
        TWCY_GREECE = 30
        TWCY_GREENLAND = 299
        TWCY_GRENADA = 8099
        TWCY_GRENEDINES = 8015
        TWCY_GUADELOUPE = 590
        TWCY_GUAM = 671
        TWCY_GUANTANAMOBAY = 5399
        TWCY_GUATEMALA = 502
        TWCY_GUINEA = 224
        TWCY_GUINEABISSAU = 1017
        TWCY_GUYANA = 592
        TWCY_HAITI = 509
        TWCY_HONDURAS = 504
        TWCY_HONGKONG = 852
        TWCY_HUNGARY = 36
        TWCY_ICELAND = 354
        TWCY_INDIA = 91
        TWCY_INDONESIA = 62
        TWCY_IRAN = 98
        TWCY_IRAQ = 964
        TWCY_IRELAND = 353
        TWCY_ISRAEL = 972
        TWCY_ITALY = 39
        TWCY_IVORYCOAST = 225
        TWCY_JAMAICA = 8010
        TWCY_JAPAN = 81
        TWCY_JORDAN = 962
        TWCY_KENYA = 254
        TWCY_KIRIBATI = 1018
        TWCY_KOREA = 82
        TWCY_KUWAIT = 965
        TWCY_LAOS = 1019
        TWCY_LEBANON = 1020
        TWCY_LIBERIA = 231
        TWCY_LIBYA = 218
        TWCY_LIECHTENSTEIN = 41
        TWCY_LUXENBOURG = 352
        TWCY_MACAO = 853
        TWCY_MADAGASCAR = 1021
        TWCY_MALAWI = 265
        TWCY_MALAYSIA = 60
        TWCY_MALDIVES = 960
        TWCY_MALI = 1022
        TWCY_MALTA = 356
        TWCY_MARSHALLIS = 692
        TWCY_MAURITANIA = 1023
        TWCY_MAURITIUS = 230
        TWCY_MEXICO = 3
        TWCY_MICRONESIA = 691
        TWCY_MIQUELON = 508
        TWCY_MONACO = 33
        TWCY_MONGOLIA = 1024
        TWCY_MONTSERRAT = 8011
        TWCY_MOROCCO = 212
        TWCY_MOZAMBIQUE = 1025
        TWCY_NAMIBIA = 264
        TWCY_NAURU = 1026
        TWCY_NEPAL = 977
        TWCY_NETHERLANDS = 31
        TWCY_NETHANTILLES = 599
        TWCY_NEVIS = 8012
        TWCY_NEWCALEDONIA = 687
        TWCY_NEWZEALAND = 64
        TWCY_NICARAGUA = 505
        TWCY_NIGER = 227
        TWCY_NIGERIA = 234
        TWCY_NIUE = 1027
        TWCY_NORFOLKI = 1028
        TWCY_NORWAY = 47
        TWCY_OMAN = 968
        TWCY_PAKISTAN = 92
        TWCY_PALAU = 1029
        TWCY_PANAMA = 507
        TWCY_PARAGUAY = 595
        TWCY_PERU = 51
        TWCY_PHILLIPPINES = 63
        TWCY_PITCAIRNIS = 1030
        TWCY_PNEWGUINEA = 675
        TWCY_POLAND = 48
        TWCY_PORTUGAL = 351
        TWCY_QATAR = 974
        TWCY_REUNIONI = 1031
        TWCY_ROMANIA = 40
        TWCY_RWANDA = 250
        TWCY_SAIPAN = 670
        TWCY_SANMARINO = 39
        TWCY_SAOTOME = 1033
        TWCY_SAUDIARABIA = 966
        TWCY_SENEGAL = 221
        TWCY_SEYCHELLESIS = 1034
        TWCY_SIERRALEONE = 1035
        TWCY_SINGAPORE = 65
        TWCY_SOLOMONIS = 1036
        TWCY_SOMALI = 1037
        TWCY_SOUTHAFRICA = 27
        TWCY_SPAIN = 34
        TWCY_SRILANKA = 94
        TWCY_STHELENA = 1032
        TWCY_STKITTS = 8013
        TWCY_STLUCIA = 8014
        TWCY_STPIERRE = 508
        TWCY_STVINCENT = 8015
        TWCY_SUDAN = 1038
        TWCY_SURINAME = 597
        TWCY_SWAZILAND = 268
        TWCY_SWEDEN = 46
        TWCY_SWITZERLAND = 41
        TWCY_SYRIA = 1039
        TWCY_TAIWAN = 886
        TWCY_TANZANIA = 255
        TWCY_THAILAND = 66
        TWCY_TOBAGO = 8016
        TWCY_TOGO = 228
        TWCY_TONGAIS = 676
        TWCY_TRINIDAD = 8016
        TWCY_TUNISIA = 216
        TWCY_TURKEY = 90
        TWCY_TURKSCAICOS = 8017
        TWCY_TUVALU = 1040
        TWCY_UGANDA = 256
        TWCY_USSR = 7
        TWCY_UAEMIRATES = 971
        TWCY_UNITEDKINGDOM = 44
        TWCY_USA = 1
        TWCY_URUGUAY = 598
        TWCY_VANUATU = 1041
        TWCY_VATICANCITY = 39
        TWCY_VENEZUELA = 58
        TWCY_WAKE = 1042
        TWCY_WALLISIS = 1043
        TWCY_WESTERNSAHARA = 1044
        TWCY_WESTERNSAMOA = 1045
        TWCY_YEMEN = 1046
        TWCY_YUGOSLAVIA = 38
        TWCY_ZAIRE = 243
        TWCY_ZAMBIA = 260
        TWCY_ZIMBABWE = 263
        '/* Added for 1.8 */
        TWCY_ALBANIA = 355
        TWCY_ARMENIA = 374
        TWCY_AZERBAIJAN = 994
        TWCY_BELARUS = 375
        TWCY_BOSNIAHERZGO = 387
        TWCY_CAMBODIA = 855
        TWCY_CROATIA = 385
        TWCY_CZECHREPUBLIC = 420
        TWCY_DIEGOGARCIA = 246
        TWCY_ERITREA = 291
        TWCY_ESTONIA = 372
        TWCY_GEORGIA = 995
        TWCY_LATVIA = 371
        TWCY_LESOTHO = 266
        TWCY_LITHUANIA = 370
        TWCY_MACEDONIA = 389
        TWCY_MAYOTTEIS = 269
        TWCY_MOLDOVA = 373
        TWCY_MYANMAR = 95
        TWCY_NORTHKOREA = 850
        TWCY_PUERTORICO = 787
        TWCY_RUSSIA = 7
        TWCY_SERBIA = 381
        TWCY_SLOVAKIA = 421
        TWCY_SLOVENIA = 386
        TWCY_SOUTHKOREA = 82
        TWCY_UKRAINE = 380
        TWCY_USVIRGINIS = 340
        TWCY_VIETNAM = 84
    End Enum
    '/****************************************************************************
    ' * Language Constants                                                       *
    ' ****************************************************************************/
    Public Enum TWLG As UInt16
        '/* Added for 1.8 */
        'TWLG_USERLOCALE = -1
        TWLG_DANISH = 0    '/* Danish                 */
        TWLG_DUTCH = 1    '/* Dutch                  */
        TWLG_ENGLISH = 2    '/* International English  */
        TWLG_FRENCH_CANADIAN = 3    '/* French Canadian        */
        TWLG_FINNISH = 4    '/* Finnish                */
        TWLG_FRENCH = 5    '/* French                 */
        TWLG_GERMAN = 6    '/* German                 */
        TWLG_ICELANDIC = 7    '/* Icelandic              */
        TWLG_ITALIAN = 8    '/* Italian                */
        TWLG_NORWEGIAN = 9    '/* Norwegian              */
        TWLG_PORTUGUESE = 10   '/* Portuguese             */
        TWLG_SPANISH = 11   '/* Spanish                */
        TWLG_SWEDISH = 12   '/* Swedish                */
        TWLG_ENGLISH_USA = 13   '/* U.S. English           */
        TWLG_AFRIKAANS = 14
        TWLG_ALBANIA = 15
        TWLG_ARABIC = 16
        TWLG_ARABIC_ALGERIA = 17
        TWLG_ARABIC_BAHRAIN = 18
        TWLG_ARABIC_EGYPT = 19
        TWLG_ARABIC_IRAQ = 20
        TWLG_ARABIC_JORDAN = 21
        TWLG_ARABIC_KUWAIT = 22
        TWLG_ARABIC_LEBANON = 23
        TWLG_ARABIC_LIBYA = 24
        TWLG_ARABIC_MOROCCO = 25
        TWLG_ARABIC_OMAN = 26
        TWLG_ARABIC_QATAR = 27
        TWLG_ARABIC_SAUDIARABIA = 28
        TWLG_ARABIC_SYRIA = 29
        TWLG_ARABIC_TUNISIA = 30
        TWLG_ARABIC_UAE = 31 '/* United Arabic Emirates */
        TWLG_ARABIC_YEMEN = 32
        TWLG_BASQUE = 33
        TWLG_BYELORUSSIAN = 34
        TWLG_BULGARIAN = 35
        TWLG_CATALAN = 36
        TWLG_CHINESE = 37
        TWLG_CHINESE_HONGKONG = 38
        TWLG_CHINESE_PRC = 39 '/* People's Republic of China */
        TWLG_CHINESE_SINGAPORE = 40
        TWLG_CHINESE_SIMPLIFIED = 41
        TWLG_CHINESE_TAIWAN = 42
        TWLG_CHINESE_TRADITIONAL = 43
        TWLG_CROATIA = 44
        TWLG_CZECH = 45
        TWLG_DUTCH_BELGIAN = 46
        TWLG_ENGLISH_AUSTRALIAN = 47
        TWLG_ENGLISH_CANADIAN = 48
        TWLG_ENGLISH_IRELAND = 49
        TWLG_ENGLISH_NEWZEALAND = 50
        TWLG_ENGLISH_SOUTHAFRICA = 51
        TWLG_ENGLISH_UK = 52
        TWLG_ESTONIAN = 53
        TWLG_FAEROESE = 54
        TWLG_FARSI = 55
        TWLG_FRENCH_BELGIAN = 56
        TWLG_FRENCH_LUXEMBOURG = 57
        TWLG_FRENCH_SWISS = 58
        TWLG_GERMAN_AUSTRIAN = 59
        TWLG_GERMAN_LUXEMBOURG = 60
        TWLG_GERMAN_LIECHTENSTEIN = 61
        TWLG_GERMAN_SWISS = 62
        TWLG_GREEK = 63
        TWLG_HEBREW = 64
        TWLG_HUNGARIAN = 65
        TWLG_INDONESIAN = 66
        TWLG_ITALIAN_SWISS = 67
        TWLG_JAPANESE = 68
        TWLG_KOREAN = 69
        TWLG_KOREAN_JOHAB = 70
        TWLG_LATVIAN = 71
        TWLG_LITHUANIAN = 72
        TWLG_NORWEGIAN_BOKMAL = 73
        TWLG_NORWEGIAN_NYNORSK = 74
        TWLG_POLISH = 75
        TWLG_PORTUGUESE_BRAZIL = 76
        TWLG_ROMANIAN = 77
        TWLG_RUSSIAN = 78
        TWLG_SERBIAN_LATIN = 79
        TWLG_SLOVAK = 80
        TWLG_SLOVENIAN = 81
        TWLG_SPANISH_MEXICAN = 82
        TWLG_SPANISH_MODERN = 83
        TWLG_THAI = 84
        TWLG_TURKISH = 85
        TWLG_UKRANIAN = 86
        '/* More stuff added for 1.8 */
        TWLG_ASSAMESE = 87
        TWLG_BENGALI = 88
        TWLG_BIHARI = 89
        TWLG_BODO = 90
        TWLG_DOGRI = 91
        TWLG_GUJARATI = 92
        TWLG_HARYANVI = 93
        TWLG_HINDI = 94
        TWLG_KANNADA = 95
        TWLG_KASHMIRI = 96
        TWLG_MALAYALAM = 97
        TWLG_MARATHI = 98
        TWLG_MARWARI = 99
        TWLG_MEGHALAYAN = 100
        TWLG_MIZO = 101
        TWLG_NAGA = 102
        TWLG_ORISSI = 103
        TWLG_PUNJABI = 104
        TWLG_PUSHTU = 105
        TWLG_SERBIAN_CYRILLIC = 106
        TWLG_SIKKIMI = 107
        TWLG_SWEDISH_FINLAND = 108
        TWLG_TAMIL = 109
        TWLG_TELUGU = 110
        TWLG_TRIPURI = 111
        TWLG_URDU = 112
        TWLG_VIETNAMESE = 113
    End Enum

    '/****************************************************************************
    ' * Data Groups                                                              *
    ' ****************************************************************************/

    '/* More Data Groups may be added in the future.
    ' * Possible candidates include text, vector graphics, sound, etc.
    ' * NOTE: Data Group constants must be powers of 2 as they are used
    ' *       as bitflags when Application asks DSM to present a list of DSs.
    ' */
    Public Enum DG As UInt32
        DG_CONTROL = &H1    '/* data pertaining to control       */
        DG_IMAGE = &H2      '/* data pertaining to raster images */
        '/* Added 1.8 */
        DG_AUDIO = &H4      '/* data pertaining to audio */
    End Enum

    '/* More Data Functionality may be added in the future.
    ' * These are for items that need to be determined before DS is opened.
    ' * NOTE: Supported Functionality constants must be powers of 2 as they are
    ' *       used as bitflags when Application asks DSM to present a list of DSs.
    ' *       to support backward capability the App and DS will not use the fields
    ' */
    Public Enum DF As UInt32
        DF_DSM2 = &H10000000    '/* added to the identity by the DSM  */
        DF_APP2 = &H20000000   '/* Set by the App to indicate it would 
        '                                             prefer to use DSM2 */
        DF_DS2 = &H40000000   '/* Set by the DS to indicate it would 
        '                                             prefer to use DSM2 */
        DG_MASK = &HFFFF       '/* all Data Groups limited to 16 bit.  Added for 2.1  */
    End Enum

    '/****************************************************************************
    ' *                                                        *
    ' ****************************************************************************/
    Public Enum DAT As UInt32
        DAT_NULL = 0                    '/* No data or structure. */
        DAT_CUSTOMBASE = &H8000         '/* Base of custom DATs.  */
        '/* Data Argument Types for the DG_CONTROL Data Group. */
        DAT_CAPABILITY = &H1            '/* TW_CAPABILITY                        */
        DAT_EVENT = &H2                 '/* TW_EVENT                             */
        DAT_IDENTITY = &H3              '/* TW_IDENTITY                          */
        DAT_PARENT = &H4                '/* TW_HANDLE, application win handle in Windows */
        DAT_PENDINGXFERS = &H5          '/* TW_PENDINGXFERS                      */
        DAT_SETUPMEMXFER = &H6          '/* TW_SETUPMEMXFER                      */
        DAT_SETUPFILEXFER = &H7         '/* TW_SETUPFILEXFER                     */
        DAT_STATUS = &H8                '/* TW_STATUS                            */
        DAT_USERINTERFACE = &H9         '/* TW_USERINTERFACE                     */
        DAT_XFERGROUP = &HA             '/* TW_UINT32                            */
        DAT_CUSTOMDSDATA = &HC          '/* TW_CUSTOMDSDATA.                     */
        DAT_DEVICEEVENT = &HD           '/* TW_DEVICEEVENT     Added 1.8         */
        DAT_FILESYSTEM = &HE            '/* TW_FILESYSTEM      Added 1.8         */
        DAT_PASSTHRU = &HF              '/* TW_PASSTHRU        Added 1.8         */
        DAT_CALLBACK = &H10             '/* TW_CALLBACK        Added 2.0         */
        DAT_STATUSUTF8 = &H11           '/* TW_STATUSUTF8      Added 2.1         */

        ''/* Data Argument Types for the DG_IMAGE Data Group. */
        DAT_IMAGEINFO = &H101           '/* TW_IMAGEINFO                         */
        DAT_IMAGELAYOUT = &H102         '/* TW_IMAGELAYOUT                       */
        DAT_IMAGEMEMXFER = &H103        '/* TW_IMAGEMEMXFER                      */
        DAT_IMAGENATIVEXFER = &H104     '/* TW_UINT32 loword is hDIB, PICHandle  */
        DAT_IMAGEFILEXFER = &H105       '/* Null data                            */
        DAT_CIECOLOR = &H106            '/* TW_CIECOLOR                          */
        DAT_GRAYRESPONSE = &H107        '/* TW_GRAYRESPONSE                      */
        DAT_RGBRESPONSE = &H108         '/* TW_RGBRESPONSE                       */
        DAT_JPEGCOMPRESSION = &H109     '/* TW_JPEGCOMPRESSION                   */
        DAT_PALETTE8 = &H10A            '/* TW_PALETTE8                          */
        DAT_EXTIMAGEINFO = &H10B        '/* TW_EXTIMAGEINFO -- for 1.7 Spec.     */

        ''/* Data Argument Types for the DG_AUDIO Data Group. */
        DAT_AUDIOFILEXFER = &H201       '/* Null data          Added 1.8         */
        DAT_AUDIOINFO = &H202           '/* TW_AUDIOINFO       Added 1.8         */
        DAT_AUDIONATIVEXFER = &H203     '/* TW_UINT32 handle to WAV, (AIFF Mac) Added 1.8 */

        ''/* misplaced */
        DAT_ICCPROFILE = &H401          '/* TW_MEMORY        Added 1.91  This Data Argument is misplaced but belongs to the DG_IMAGE Data Group */
        DAT_IMAGEMEMFILEXFER = &H402    '/* TW_IMAGEMEMXFER  Added 1.91  This Data Argument is misplaced but belongs to the DG_IMAGE Data Group */
        DAT_ENTRYPOINT = &H403          '/* TW_ENTRYPOINT    Added 2.0   This Data Argument is misplaced but belongs to the DG_CONTROL Data Group */
    End Enum

    '/****************************************************************************
    ' * Messages                                                                 *
    ' ****************************************************************************/

    '/* All message constants are unique.
    ' * Messages are grouped according to which DATs they are used with.*/
    Public Enum MSG As UInt16
        MSG_NULL = &H0 '/* Used in TW_EVENT structure               */
        MSG_CUSTOMBASE = &H8000 '/* Base of custom messages                  */

        ''/* Generic messages may be used with any of several DATs.                   */
        MSG_GET = &H1 '/* Get one or more values                   */
        MSG_GETCURRENT = &H2 '/* Get current value                        */
        MSG_GETDEFAULT = &H3 '/* Get default (e.g. power up) value        */
        MSG_GETFIRST = &H4 '/* Get first of a series of items, e.g. DSs */
        MSG_GETNEXT = &H5 '/* Iterate through a series of items.       */
        MSG_SET = &H6 '/* Set one or more values                   */
        MSG_RESET = &H7 '/* Set current value to default value       */
        MSG_QUERYSUPPORT = &H8 '/* Get supported operations on the cap.     */
        MSG_GETHELP = &H9 '/* Returns help text suitable for use in a GUI        Added 2.1 */
        MSG_GETLABEL = &HA '/* Returns a label suitable for use in a GUI          Added 2.1 */
        MSG_GETLABELENUM = &HB '/* Return all of the labels for a capability of type  Added 2.1 */


        ''/* Messages used with DAT_NULL                                              */
        MSG_XFERREADY = &H101 '/* The data source has data ready           */
        MSG_CLOSEDSREQ = &H102 '/* Request for Application. to close DS               */
        MSG_CLOSEDSOK = &H103 '/* Tell the Application. to save the state.           */
        MSG_DEVICEEVENT = &H104 '/* Some event has taken place               Added 1.8 */

        ''/* Messages used with a pointer to DAT_PARENT data                          */
        MSG_OPENDSM = &H301 '/* Open the DSM                             */
        MSG_CLOSEDSM = &H302 '/* Close the DSM                            */

        ''/* Messages used with a pointer to a DAT_IDENTITY structure                 */
        MSG_OPENDS = &H401 '/* Open a data source                       */
        MSG_CLOSEDS = &H402 '/* Close a data source                      */
        MSG_USERSELECT = &H403 '/* Put up a dialog of all DS                */

        ''/* Messages used with a pointer to a DAT_USERINTERFACE structure            */
        MSG_DISABLEDS = &H501 '/* Disable data transfer in the DS          */
        MSG_ENABLEDS = &H502 '/* Enable data transfer in the DS           */
        MSG_ENABLEDSUIONLY = &H503  '/* Enable for saving DS state only.     */

        ''/* Messages used with a pointer to a DAT_EVENT structure                    */
        MSG_PROCESSEVENT = &H601

        ''/* Messages used with a pointer to a DAT_PENDINGXFERS structure             */
        MSG_ENDXFER = &H701
        MSG_STOPFEEDER = &H702

        ''/* Messages used with a pointer to a DAT_FILESYSTEM structure               */
        MSG_CHANGEDIRECTORY = &H801 '/* Added 1.8 */
        MSG_CREATEDIRECTORY = &H802 '/* Added 1.8 */
        MSG_DELETE = &H803 '/* Added 1.8 */
        MSG_FORMATMEDIA = &H804 '/* Added 1.8 */
        MSG_GETCLOSE = &H805 '/* Added 1.8 */
        MSG_GETFIRSTFILE = &H806 '/* Added 1.8 */
        MSG_GETINFO = &H807 '/* Added 1.8 */
        MSG_GETNEXTFILE = &H808 '/* Added 1.8 */
        MSG_RENAME = &H809 '/* Added 1.8 */
        MSG_COPY = &H80A '/* Added 1.8 */
        MSG_AUTOMATICCAPTUREDIRECTORY = &H80B '/* Added 1.8 */

        ''/* Messages used with a pointer to a DAT_PASSTHRU structure                 */
        MSG_PASSTHRU = &H901

        ''/* used with DAT_CALLBACK */
        MSG_REGISTER_CALLBACK = &H902

        ''/* used with DAT_CAPABILITY */
        MSG_RESETALL = &HA01 '/*  Added 1.91 */
    End Enum
    '/****************************************************************************
    ' * Capabilities                                                             *
    ' ****************************************************************************/
    Public Enum CAP As UInt16
        CAP_CUSTOMBASE = &H8000 '/* Base of custom capabilities */

        ''/* all data sources are REQUIRED to support these caps */
        CAP_XFERCOUNT = &H1

        ''/* image data sources are REQUIRED to support these caps */
        ICAP_COMPRESSION = &H100
        ICAP_PIXELTYPE = &H101
        ICAP_UNITS = &H102 '/* default is TWUN_INCHES */
        ICAP_XFERMECH = &H103

        ''/* all data sources MAY support these caps */
        CAP_AUTHOR = &H1000
        CAP_CAPTION = &H1001
        CAP_FEEDERENABLED = &H1002
        CAP_FEEDERLOADED = &H1003
        CAP_TIMEDATE = &H1004
        CAP_SUPPORTEDCAPS = &H1005
        CAP_EXTENDEDCAPS = &H1006
        CAP_AUTOFEED = &H1007
        CAP_CLEARPAGE = &H1008
        CAP_FEEDPAGE = &H1009
        CAP_REWINDPAGE = &H100A
        CAP_INDICATORS = &H100B   '/* Added 1.1 */
        CAP_PAPERDETECTABLE = &H100D   '/* Added 1.6 */
        CAP_UICONTROLLABLE = &H100E   '/* Added 1.6 */
        CAP_DEVICEONLINE = &H100F   '/* Added 1.6 */
        CAP_AUTOSCAN = &H1010   '/* Added 1.6 */
        CAP_THUMBNAILSENABLED = &H1011   '/* Added 1.7 */
        CAP_DUPLEX = &H1012   '/* Added 1.7 */
        CAP_DUPLEXENABLED = &H1013   '/* Added 1.7 */
        CAP_ENABLEDSUIONLY = &H1014   '/* Added 1.7 */
        CAP_CUSTOMDSDATA = &H1015   '/* Added 1.7 */
        CAP_ENDORSER = &H1016   '/* Added 1.7 */
        CAP_JOBCONTROL = &H1017   '/* Added 1.7 */
        CAP_ALARMS = &H1018   '/* Added 1.8 */
        CAP_ALARMVOLUME = &H1019   '/* Added 1.8 */
        CAP_AUTOMATICCAPTURE = &H101A   '/* Added 1.8 */
        CAP_TIMEBEFOREFIRSTCAPTURE = &H101B   '/* Added 1.8 */
        CAP_TIMEBETWEENCAPTURES = &H101C   '/* Added 1.8 */
        CAP_CLEARBUFFERS = &H101D   '/* Added 1.8 */
        CAP_MAXBATCHBUFFERS = &H101E   '/* Added 1.8 */
        CAP_DEVICETIMEDATE = &H101F   '/* Added 1.8 */
        CAP_POWERSUPPLY = &H1020   '/* Added 1.8 */
        CAP_CAMERAPREVIEWUI = &H1021   '/* Added 1.8 */
        CAP_DEVICEEVENT = &H1022   '/* Added 1.8 */
        CAP_SERIALNUMBER = &H1024   '/* Added 1.8 */
        CAP_PRINTER = &H1026   '/* Added 1.8 */
        CAP_PRINTERENABLED = &H1027   '/* Added 1.8 */
        CAP_PRINTERINDEX = &H1028   '/* Added 1.8 */
        CAP_PRINTERMODE = &H1029   '/* Added 1.8 */
        CAP_PRINTERSTRING = &H102A   '/* Added 1.8 */
        CAP_PRINTERSUFFIX = &H102B   '/* Added 1.8 */
        CAP_LANGUAGE = &H102C   '/* Added 1.8 */
        CAP_FEEDERALIGNMENT = &H102D   '/* Added 1.8 */
        CAP_FEEDERORDER = &H102E   '/* Added 1.8 */
        CAP_REACQUIREALLOWED = &H1030   '/* Added 1.8 */
        CAP_BATTERYMINUTES = &H1032   '/* Added 1.8 */
        CAP_BATTERYPERCENTAGE = &H1033   '/* Added 1.8 */
        CAP_CAMERASIDE = &H1034   '/* Added 1.91 */
        CAP_SEGMENTED = &H1035   '/* Added 1.91 */
        CAP_CAMERAENABLED = &H1036   '/* Added 2.0 */
        CAP_CAMERAORDER = &H1037   '/* Added 2.0 */
        CAP_MICRENABLED = &H1038   '/* Added 2.0 */
        CAP_FEEDERPREP = &H1039   '/* Added 2.0 */
        CAP_FEEDERPOCKET = &H103A   '/* Added 2.0 */
        CAP_AUTOMATICSENSEMEDIUM = &H103B   '/* Added 2.1 */
        CAP_CUSTOMINTERFACEGUID = &H103C   '/* Added 2.1 */


        ''/* image data sources MAY support these caps */
        ICAP_AUTOBRIGHT = &H1100
        ICAP_BRIGHTNESS = &H1101
        ICAP_CONTRAST = &H1103
        ICAP_CUSTHALFTONE = &H1104
        ICAP_EXPOSURETIME = &H1105
        ICAP_FILTER = &H1106
        ICAP_FLASHUSED = &H1107
        ICAP_GAMMA = &H1108
        ICAP_HALFTONES = &H1109
        ICAP_HIGHLIGHT = &H110A
        ICAP_IMAGEFILEFORMAT = &H110C
        ICAP_LAMPSTATE = &H110D
        ICAP_LIGHTSOURCE = &H110E
        ICAP_ORIENTATION = &H1110
        ICAP_PHYSICALWIDTH = &H1111
        ICAP_PHYSICALHEIGHT = &H1112
        ICAP_SHADOW = &H1113
        ICAP_FRAMES = &H1114
        ICAP_XNATIVERESOLUTION = &H1116
        ICAP_YNATIVERESOLUTION = &H1117
        ICAP_XRESOLUTION = &H1118
        ICAP_YRESOLUTION = &H1119
        ICAP_MAXFRAMES = &H111A
        ICAP_TILES = &H111B
        ICAP_BITORDER = &H111C
        ICAP_CCITTKFACTOR = &H111D
        ICAP_LIGHTPATH = &H111E
        ICAP_PIXELFLAVOR = &H111F
        ICAP_PLANARCHUNKY = &H1120
        ICAP_ROTATION = &H1121
        ICAP_SUPPORTEDSIZES = &H1122
        ICAP_THRESHOLD = &H1123
        ICAP_XSCALING = &H1124
        ICAP_YSCALING = &H1125
        ICAP_BITORDERCODES = &H1126
        ICAP_PIXELFLAVORCODES = &H1127
        ICAP_JPEGPIXELTYPE = &H1128
        ICAP_TIMEFILL = &H112A
        ICAP_BITDEPTH = &H112B
        ICAP_BITDEPTHREDUCTION = &H112C  '/* Added 1.5 */
        ICAP_UNDEFINEDIMAGESIZE = &H112D  '/* Added 1.6 */
        ICAP_IMAGEDATASET = &H112E  '/* Added 1.7 */
        ICAP_EXTIMAGEINFO = &H112F  '/* Added 1.7 */
        ICAP_MINIMUMHEIGHT = &H1130  '/* Added 1.7 */
        ICAP_MINIMUMWIDTH = &H1131  '/* Added 1.7 */
        ICAP_AUTODISCARDBLANKPAGES = &H1134  '/* Added 2.0 */
        ICAP_FLIPROTATION = &H1136  '/* Added 1.8 */
        ICAP_BARCODEDETECTIONENABLED = &H1137  '/* Added 1.8 */
        ICAP_SUPPORTEDBARCODETYPES = &H1138  '/* Added 1.8 */
        ICAP_BARCODEMAXSEARCHPRIORITIES = &H1139  '/* Added 1.8 */
        ICAP_BARCODESEARCHPRIORITIES = &H113A  '/* Added 1.8 */
        ICAP_BARCODESEARCHMODE = &H113B  '/* Added 1.8 */
        ICAP_BARCODEMAXRETRIES = &H113C  '/* Added 1.8 */
        ICAP_BARCODETIMEOUT = &H113D  '/* Added 1.8 */
        ICAP_ZOOMFACTOR = &H113E  '/* Added 1.8 */
        ICAP_PATCHCODEDETECTIONENABLED = &H113F  '/* Added 1.8 */
        ICAP_SUPPORTEDPATCHCODETYPES = &H1140  '/* Added 1.8 */
        ICAP_PATCHCODEMAXSEARCHPRIORITIES = &H1141  '/* Added 1.8 */
        ICAP_PATCHCODESEARCHPRIORITIES = &H1142  '/* Added 1.8 */
        ICAP_PATCHCODESEARCHMODE = &H1143  '/* Added 1.8 */
        ICAP_PATCHCODEMAXRETRIES = &H1144  '/* Added 1.8 */
        ICAP_PATCHCODETIMEOUT = &H1145  '/* Added 1.8 */
        ICAP_FLASHUSED2 = &H1146  '/* Added 1.8 */
        ICAP_IMAGEFILTER = &H1147  '/* Added 1.8 */
        ICAP_NOISEFILTER = &H1148  '/* Added 1.8 */
        ICAP_OVERSCAN = &H1149  '/* Added 1.8 */
        ICAP_AUTOMATICBORDERDETECTION = &H1150  '/* Added 1.8 */
        ICAP_AUTOMATICDESKEW = &H1151  '/* Added 1.8 */
        ICAP_AUTOMATICROTATE = &H1152  '/* Added 1.8 */
        ICAP_JPEGQUALITY = &H1153  '/* Added 1.9 */
        ICAP_FEEDERTYPE = &H1154  '/* Added 1.91 */
        ICAP_ICCPROFILE = &H1155  '/* Added 1.91 */
        ICAP_AUTOSIZE = &H1156  '/* Added 2.0 */
        ICAP_AUTOMATICCROPUSESFRAME = &H1157  '/* Added 2.1 */
        ICAP_AUTOMATICLENGTHDETECTION = &H1158  '/* Added 2.1 */
        ICAP_AUTOMATICCOLORENABLED = &H1159  '/* Added 2.1 */
        ICAP_AUTOMATICCOLORNONCOLORPIXELTYPE = &H115A  '/* Added 2.1 */
        ICAP_COLORMANAGEMENTENABLED = &H115B  '/* Added 2.1 */
        ICAP_IMAGEMERGE = &H115C  '/* Added 2.1 */
        ICAP_IMAGEMERGEHEIGHTTHRESHOLD = &H115D  '/* Added 2.1 */
        ICAP_SUPPORTEDEXTIMAGEINFO = &H115E  '/* Added 2.1 */

        ''/* image data sources MAY support these audio caps */
        ACAP_XFERMECH = &H1202  '/* Added 1.8 */
    End Enum

    '/***************************************************************************
    ' *            Extended Image Info Attributes section  Added 1.7            *
    ' ***************************************************************************/

    '#define TWEI_BARCODEX               0x1200
    '#define TWEI_BARCODEY               0x1201
    '#define TWEI_BARCODETEXT            0x1202
    '#define TWEI_BARCODETYPE            0x1203
    '#define TWEI_DESHADETOP             0x1204
    '#define TWEI_DESHADELEFT            0x1205
    '#define TWEI_DESHADEHEIGHT          0x1206
    '#define TWEI_DESHADEWIDTH           0x1207
    '#define TWEI_DESHADESIZE            0x1208
    '#define TWEI_SPECKLESREMOVED        0x1209
    '#define TWEI_HORZLINEXCOORD         0x120A
    '#define TWEI_HORZLINEYCOORD         0x120B
    '#define TWEI_HORZLINELENGTH         0x120C
    '#define TWEI_HORZLINETHICKNESS      0x120D
    '#define TWEI_VERTLINEXCOORD         0x120E
    '#define TWEI_VERTLINEYCOORD         0x120F
    '#define TWEI_VERTLINELENGTH         0x1210
    '#define TWEI_VERTLINETHICKNESS      0x1211
    '#define TWEI_PATCHCODE              0x1212
    '#define TWEI_ENDORSEDTEXT           0x1213
    '#define TWEI_FORMCONFIDENCE         0x1214
    '#define TWEI_FORMTEMPLATEMATCH      0x1215
    '#define TWEI_FORMTEMPLATEPAGEMATCH  0x1216
    '#define TWEI_FORMHORZDOCOFFSET      0x1217
    '#define TWEI_FORMVERTDOCOFFSET      0x1218
    '#define TWEI_BARCODECOUNT           0x1219
    '#define TWEI_BARCODECONFIDENCE      0x121A
    '#define TWEI_BARCODEROTATION        0x121B
    '#define TWEI_BARCODETEXTLENGTH      0x121C
    '#define TWEI_DESHADECOUNT           0x121D
    '#define TWEI_DESHADEBLACKCOUNTOLD   0x121E
    '#define TWEI_DESHADEBLACKCOUNTNEW   0x121F
    '#define TWEI_DESHADEBLACKRLMIN      0x1220
    '#define TWEI_DESHADEBLACKRLMAX      0x1221
    '#define TWEI_DESHADEWHITECOUNTOLD   0x1222
    '#define TWEI_DESHADEWHITECOUNTNEW   0x1223
    '#define TWEI_DESHADEWHITERLMIN      0x1224
    '#define TWEI_DESHADEWHITERLAVE      0x1225
    '#define TWEI_DESHADEWHITERLMAX      0x1226
    '#define TWEI_BLACKSPECKLESREMOVED   0x1227
    '#define TWEI_WHITESPECKLESREMOVED   0x1228
    '#define TWEI_HORZLINECOUNT          0x1229
    '#define TWEI_VERTLINECOUNT          0x122A
    '#define TWEI_DESKEWSTATUS           0x122B
    '#define TWEI_SKEWORIGINALANGLE      0x122C
    '#define TWEI_SKEWFINALANGLE         0x122D
    '#define TWEI_SKEWCONFIDENCE         0x122E
    '#define TWEI_SKEWWINDOWX1           0x122F
    '#define TWEI_SKEWWINDOWY1           0x1230
    '#define TWEI_SKEWWINDOWX2           0x1231
    '#define TWEI_SKEWWINDOWY2           0x1232
    '#define TWEI_SKEWWINDOWX3           0x1233
    '#define TWEI_SKEWWINDOWY3           0x1234
    '#define TWEI_SKEWWINDOWX4           0x1235
    '#define TWEI_SKEWWINDOWY4           0x1236
    '#define TWEI_BOOKNAME               0x1238  /* added 1.9 */
    '#define TWEI_CHAPTERNUMBER          0x1239  /* added 1.9 */
    '#define TWEI_DOCUMENTNUMBER         0x123A  /* added 1.9 */
    '#define TWEI_PAGENUMBER             0x123B  /* added 1.9 */
    '#define TWEI_CAMERA                 0x123C  /* added 1.9 */
    '#define TWEI_FRAMENUMBER            0x123D  /* added 1.9 */
    '#define TWEI_FRAME                  0x123E  /* added 1.9 */
    '#define TWEI_PIXELFLAVOR            0x123F  /* added 1.9 */
    '#define TWEI_ICCPROFILE             0x1240  /* added 1.91 */
    '#define TWEI_LASTSEGMENT            0x1241  /* added 1.91 */
    '#define TWEI_SEGMENTNUMBER          0x1242  /* added 1.91 */
    '#define TWEI_MAGDATA                0x1243  /* added 2.0 */
    '#define TWEI_MAGTYPE                0x1244  /* added 2.0 */
    '#define TWEI_PAGESIDE               0x1245  /* added 2.0 */
    '#define TWEI_FILESYSTEMSOURCE       0x1246  /* added 2.0 */
    '#define TWEI_IMAGEMERGED            0x1247  /* added 2.1 */
    '#define TWEI_MAGDATALENGTH          0x1248  /* added 2.1 */

    '#define TWEJ_NONE                   0x0000
    '#define TWEJ_MIDSEPARATOR           0x0001
    '#define TWEJ_PATCH1                 0x0002
    '#define TWEJ_PATCH2                 0x0003
    '#define TWEJ_PATCH3                 0x0004
    '#define TWEJ_PATCH4                 0x0005
    '#define TWEJ_PATCH6                 0x0006
    '#define TWEJ_PATCHT                 0x0007


    '/***************************************************************************
    ' *            Return Codes and Condition Codes section                     *
    ' ***************************************************************************/

    '/* Return Codes: DSM_Entry and DS_Entry may return any one of these values. */
    Public Enum TWRC 'Enum not part of standard
        TWRC_CUSTOMBASE = &H8000
        TWRC_SUCCESS = 0
        TWRC_FAILURE = 1        '/* Application may get TW_STATUS for info on failure */
        TWRC_CHECKSTATUS = 2    '/* "tried hard"; get status                  */
        TWRC_CANCEL = 3
        TWRC_DSEVENT = 4
        TWRC_NOTDSEVENT = 5
        TWRC_XFERDONE = 6
        TWRC_ENDOFLIST = 7      '/* After MSG_GETNEXT if nothing left         */
        TWRC_INFONOTSUPPORTED = 8
        TWRC_DATANOTAVAILABLE = 9
    End Enum

    '/* Condition Codes: Application gets these by doing DG_CONTROL DAT_STATUS MSG_GET.  */
    Public Enum TWCC As Int16
        TWCC_CUSTOMBASE = &H8000S
        TWCC_SUCCESS = 0            '/* It worked!                                */
        TWCC_BUMMER = 1             '/* Failure due to unknown causes             */
        TWCC_LOWMEMORY = 2          '/* Not enough memory to perform operation    */
        TWCC_NODS = 3               '/* No Data Source                            */
        TWCC_MAXCONNECTIONS = 4     '/* DS is connected to max possible applications      */
        TWCC_OPERATIONERROR = 5     '/* DS or DSM reported error, application shouldn't   */
        TWCC_BADCAP = 6             '/* Unknown capability                        */
        TWCC_BADPROTOCOL = 9        '/* Unrecognized MSG DG DAT combination       */
        TWCC_BADVALUE = 10          '/* Data parameter out of range              */
        TWCC_SEQERROR = 11          '/* DG DAT MSG out of expected sequence      */
        TWCC_BADDEST = 12           '/* Unknown destination Application/Source in DSM_Entry */
        TWCC_CAPUNSUPPORTED = 13    '/* Capability not supported by source            */
        TWCC_CAPBADOPERATION = 14   '/* Operation not supported by capability         */
        TWCC_CAPSEQERROR = 15       '/* Capability has dependancy on other capability */
        TWCC_DENIED = 16            '/* File System operation is denied (file is protected) Added 1.8 */
        TWCC_FILEEXISTS = 17        '/* Operation failed because file already exists.       Added 1.8 */
        TWCC_FILENOTFOUND = 18      '/* File not found                                      Added 1.8 */
        TWCC_NOTEMPTY = 19          '/* Operation failed because directory is not empty     Added 1.8 */
        TWCC_PAPERJAM = 20          '/* The feeder is jammed                                Added 1.8 */
        TWCC_PAPERDOUBLEFEED = 21   '/* The feeder detected multiple pages                  Added 1.8 */
        TWCC_FILEWRITEERROR = 22    '/* Error writing the file (meant for things like disk full conditions)Added 1.8 */
        TWCC_CHECKDEVICEONLINE = 23 '/* The device went offline prior to or during this operation Added 1.8 */
        TWCC_INTERLOCK = 24         '/* Added 2.0 */
        TWCC_DAMAGEDCORNER = 25     '/* Added 2.0 */
        TWCC_FOCUSERROR = 26        '/* Added 2.0 */
        TWCC_DOCTOOLIGHT = 27       '/* Added 2.0 */
        TWCC_DOCTOODARK = 28        '/* Added 2.0 */
        TWCC_NOMEDIA = 29           '/* Added 2.1 */
    End Enum
    '/* bit patterns: for query the operation that are supported by the data source on a capability */
    '/* Application gets these through DG_CONTROL/DAT_CAPABILITY/MSG_QUERYSUPPORT */
    '/* Added 1.6 */
    Public Enum TWQC
        TWQC_GET = &H1
        TWQC_SET = &H2
        TWQC_GETDEFAULT = &H4
        TWQC_GETCURRENT = &H8
        TWQC_RESET = &H10
        TWQC_ALLGET = TWQC_GET Or TWQC_GETDEFAULT Or TWQC_GETCURRENT
        TWQC_ALL = TWQC_ALLGET Or TWQC_SET Or TWQC_RESET
    End Enum

    '/****************************************************************************
    ' * Depreciated Items                                                        *
    ' ****************************************************************************/
    '#if defined(WIN32) || defined(WIN64)
    '        #define TW_HUGE
    '#elif !defined(TWH_CMP_GNU)
    '        #define TW_HUGE    huge
    '#Else
    '        #define TW_HUGE
    '#End If



    'typedef BYTE TW_HUGE * HPBYTE;
    'typedef void TW_HUGE * HPVOID;

    'typedef unsigned char     TW_STR1024[1026],   FAR *pTW_STR1026;
    'typedef wchar_t           TW_UNI512[512],     FAR *pTW_UNI512;

    '#define TWTY_STR1024          0x000d    /* Means Item is a TW_STR1024...added 1.9 */
    '#define TWTY_UNI512           0x000e    /* Means Item is a TW_UNI512...added 1.9 */

    '#define TWFF_JPN              12        /* 1.91 */

    '#define DAT_TWUNKIDENTITY     0x000b    /* Additional message required for thunker to request the special identity information. */
    '#define DAT_SETUPFILEXFER2    0x0301    /* Data transfer via a file. deprecated - use DAT_SETUPFILEXFER instead*/

    '#define CAP_SUPPORTEDCAPSEXT  0x100c
    '#define CAP_FILESYSTEM        //0x????
    '#define ACAP_AUDIOFILEFORMAT  0x1201    /* Added 1.8 */

    '#define MSG_CHECKSTATUS       0x0201    /* Get status information - use MSG_GET instead */

    '#define MSG_INVOKE_CALLBACK   0x0903    /* Mac Only, deprecated - use DAT_NULL and MSG_xxx instead */

    '#define TWSX_FILE2            3

    '/* CAP_FILESYSTEM values (FS_ means file system) */
    '#define TWFS_FILESYSTEM       0
    '#define TWFS_RECURSIVEDELETE  1

    '/* ICAP_PIXELTYPE values (PT_ means Pixel Type) */
    '#define TWPT_SRGB64     11 /* 1.91 */
    '#define TWPT_BGR        12 /* 1.91 */
    '#define TWPT_CIELAB     13 /* 1.91 */
    '#define TWPT_CIELUV     14 /* 1.91 */
    '#define TWPT_YCBCR      15 /* 1.91 */

    '/* ICAP_SUPPORTEDSIZES values (SS_ means Supported Sizes) */
    '#define TWSS_B                8
    '#define TWSS_A4LETTER    TWSS_A4      /* use TWSS_A4 instead */
    '#define TWSS_B3          TWSS_ISOB3   /* use TWSS_ISOB3 instead */
    '#define TWSS_B4          TWSS_ISOB4   /* use TWSS_ISOB4 instead */
    '#define TWSS_B6          TWSS_ISOB6   /* use TWSS_ISOB6 instead */
    '#define TWSS_B5LETTER    TWSS_JISB5   /* use TWSS_JISB5 instead */

    '/* CAP_LANGUAGE Language Constants */
    '#define TWLG_DAN    TWLG_DANISH           /* use TWLG_DANISH instead */
    '#define TWLG_DUT    TWLG_DUTCH            /* use TWLG_DUTCH instead */
    '#define TWLG_ENG    TWLG_ENGLISH          /* use TWLG_ENGLISH instead */
    '#define TWLG_USA    TWLG_ENGLISH_USA      /* use TWLG_ENGLISH_USA instead */
    '#define TWLG_FIN    TWLG_FINNISH          /* use TWLG_FINNISH instead */
    '#define TWLG_FRN    TWLG_FRENCH           /* use TWLG_FRENCH instead */
    '#define TWLG_FCF    TWLG_FRENCH_CANADIAN  /* use TWLG_FRENCH_CANADIAN instead */
    '#define TWLG_GER    TWLG_GERMAN           /* use TWLG_GERMAN instead */
    '#define TWLG_ICE    TWLG_ICELANDIC        /* use TWLG_ICELANDIC instead */
    '#define TWLG_ITN    TWLG_ITALIAN          /* use TWLG_ITALIAN instead */
    '#define TWLG_NOR    TWLG_NORWEGIAN        /* use TWLG_NORWEGIAN instead */
    '#define TWLG_POR    TWLG_PORTUGUESE       /* use TWLG_PORTUGUESE instead */
    '#define TWLG_SPA    TWLG_SPANISH          /* use TWLG_SPANISH instead */
    '#define TWLG_SWE    TWLG_SWEDISH          /* use TWLG_SWEDISH instead */


    '/* ACAP_AUDIOFILEFORMAT values (AF_ means audio format).  Added 1.8 */
    '#define TWAF_WAV      0
    '#define TWAF_AIFF     1
    '#define TWAF_AU       3
    '#define TWAF_SND      4


    '/* DAT_SETUPFILEXFER2. Sets up DS to application data transfer via a file. Added 1.9 */
    'typedef struct {
    '   TW_MEMREF FileName;     /* Pointer to file name text */
    '   TW_UINT16 FileNameType; /* TWTY_STR1024 or TWTY_UNI512 */
    '   TW_UINT16 Format;       /* Any TWFF_ constant */
    '   TW_INT16  VRefNum;      /* Used for Mac only  */
    '   TW_UINT32 parID;        /* Used for Mac only */
    '} TW_SETUPFILEXFER2, FAR * pTW_SETUPFILEXFER2;

    '/* SDH - 03/21/95 - TWUNK */
    '/* DAT_TWUNKIDENTITY. Provides DS identity and 'other' information necessary */
    '/*                    across thunk link. */
    'typedef struct {
    '   TW_IDENTITY identity;        /* Identity of data source.                 */
    '   TW_STR255   dsPath;          /* Full path and file name of data source.  */
    '} TW_TWUNKIDENTITY, FAR * pTW_TWUNKIDENTITY;

    '/* SDH - 03/21/95 - TWUNK */
    '/* Provides DS_Entry parameters over thunk link. */
    '/*  SDH - 03/23/95 - WATCH                                                  */
    '/*  The thunker requires knowledge about size of data being passed in the   */
    '/*  lpData parameter to DS_Entry (which is not readily available due to     */
    '/*  type LPVOID.  Thus, we key off the DAT_ argument to determine the size. */
    '/*  This has a couple implications:                                         */
    '/*  1) Any additional DAT_ features require modifications to the thunk code */
    '/*     for thunker support.                                                 */
    '/*  2) Any applications which use the custom capabailites are not supported */
    '/*     under thunking since we have no way of knowing what size data (if    */
    '/*     any) is being passed.                                                */
    'typedef struct
    '{
    '    TW_INT8     destFlag;       /* TRUE if dest is not NULL                 */
    '    TW_IDENTITY dest;           /* Identity of data source (if used)        */
    '    TW_INT32    dataGroup;      /* DSM_Entry dataGroup parameter            */
    '    TW_INT16    dataArgType;    /* DSM_Entry dataArgType parameter          */
    '    TW_INT16    message;        /* DSM_Entry message parameter              */
    '    TW_INT32    pDataSize;      /* Size of pData (0 if NULL)                */
    '    //  TW_MEMREF   pData;      /* Based on implementation specifics, a     */
    '                                /* pData parameter makes no sense in this   */
    '                                /* structure, but data (if provided) will be*/
    '                                /* appended in the data block.              */
    '} TW_TWUNKDSENTRYPARAMS, FAR * pTW_TWUNKDSENTRYPARAMS;

    '/* SDH - 03/21/95 - TWUNK */
    '/* Provides DS_Entry results over thunk link. */
    'typedef struct
    '{
    '    TW_UINT16   returnCode;     /* Thunker DsEntry return code.             */
    '    TW_UINT16   conditionCode;  /* Thunker DsEntry condition code.          */
    '    TW_INT32    pDataSize;      /* Size of pData (0 if NULL)                */
    '    //  TW_MEMREF   pData;      /* Based on implementation specifics, a     */
    '                                /* pData parameter makes no sense in this   */
    '                                /* structure, but data (if provided) will be*/
    '                                /* appended in the data block.              */
    '} TW_TWUNKDSENTRYRETURN, FAR * pTW_TWUNKDSENTRYRETURN;

    '/* WJD - 950818 */
    '/* Added for 1.6 Specification */
    '/* TWAIN 1.6 CAP_SUPPORTEDCAPSEXT structure */
    'typedef struct
    '{
    '    TW_UINT16 Cap;   /* Which CAP/ICAP info is relevant to */
    '    TW_UINT16 Properties;  /* Messages this CAP/ICAP supports */
    '} TW_CAPEXT, FAR * pTW_CAPEXT;

    '/* DAT_SETUPAUDIOFILEXFER, information required to setup an audio file transfer */
    'typedef struct {
    '   TW_STR255  FileName; /* full path target file */
    '   TW_UINT16  Format;   /* one of TWAF_xxxx */
    '   TW_INT16 VRefNum;
    '} TW_SETUPAUDIOFILEXFER, FAR * pTW_SETUPAUDIOFILEXFER;


    '/****************************************************************************
    ' * Entry Points                                                             *
    ' ****************************************************************************/

    '/**********************************************************************
    ' * Function: DSM_Entry, the only entry point into the Data Source Manager.
    ' *
    ' * Parameters:
    ' *  pOrigin Identifies the source module of the message. This could
    ' *          identify an Application, a Source, or the Source Manager.
    ' *
    ' *  pDest   Identifies the destination module for the message.
    ' *          This could identify an application or a data source.
    ' *          If this is NULL, the message goes to the Source Manager.
    ' *
    ' *  DG      The Data Group. 
    ' *          Example: DG_IMAGE.
    ' *
    ' *  DAT     The Data Attribute Type.
    ' *          Example: DAT_IMAGEMEMXFER.
    ' *    
    ' *  MSG     The message.  Messages are interpreted by the destination module
    ' *          with respect to the Data Group and the Data Attribute Type.  
    ' *          Example: MSG_GET.
    ' *
    ' *  pData   A pointer to the data structure or variable identified 
    ' *          by the Data Attribute Type.
    ' *          Example: (TW_MEMREF)&ImageMemXfer
    ' *                   where ImageMemXfer is a TW_IMAGEMEMXFER structure.
    ' *                    
    ' * Returns:
    ' *  ReturnCode
    ' *         Example: TWRC_SUCCESS.
    ' *
    ' ********************************************************************/

    '/* Don't mangle the name "DSM_Entry" if we're compiling in C++! */
    '#ifdef  __cplusplus
    'extern "C" {
    '#endif  /* __cplusplus */

    '#ifdef TWH_CMP_MSC

    '  TW_UINT16 FAR PASCAL DSM_Entry( pTW_IDENTITY pOrigin,
    '                                  pTW_IDENTITY pDest,
    '                                  TW_UINT32    DG,
    '                                  TW_UINT16    DAT,
    '                                  TW_UINT16    MSG,
    '                                  TW_MEMREF    pData);

    '  typedef TW_UINT16 (FAR PASCAL *DSMENTRYPROC)(pTW_IDENTITY pOrigin,
    '                                               pTW_IDENTITY pDest,
    '                                               TW_UINT32 DG,
    '                                               TW_UINT16 DAT,
    '                                               TW_UINT16 MSG,
    '                                               TW_MEMREF pData);

    '#else

    '  FAR PASCAL TW_UINT16 DSM_Entry( pTW_IDENTITY pOrigin,
    '                                  pTW_IDENTITY pDest,
    '                                  TW_UINT32    DG,
    '                                  TW_UINT16    DAT,
    '                                  TW_UINT16    MSG,
    '                                  TW_MEMREF    pData);

    '  typedef TW_UINT16 (*DSMENTRYPROC)(pTW_IDENTITY pOrigin, 
    '                                    pTW_IDENTITY pDest,
    '                                    TW_UINT32 DG, 
    '                                    TW_UINT16 DAT, 
    '                                    TW_UINT16 MSG,
    '                                    TW_MEMREF pData);

    '#endif  /* TWH_CMP_MSC */

    '#ifdef  __cplusplus
    '}
    '#endif  /* cplusplus */


    '/**********************************************************************
    ' * Function: DS_Entry, the entry point provided by a Data Source.
    ' *
    ' * Parameters:
    ' *  pOrigin Identifies the source module of the message. This could
    ' *          identify an application or the Data Source Manager.
    ' *
    ' *  DG      The Data Group. 
    ' *          Example: DG_IMAGE.
    ' *           
    ' *  DAT     The Data Attribute Type.
    ' *          Example: DAT_IMAGEMEMXFER.
    ' *    
    ' *  MSG     The message.  Messages are interpreted by the data source
    ' *          with respect to the Data Group and the Data Attribute Type.
    ' *          Example: MSG_GET.
    ' *
    ' *  pData   A pointer to the data structure or variable identified 
    ' *          by the Data Attribute Type.
    ' *          Example: (TW_MEMREF)&ImageMemXfer
    ' *                   where ImageMemXfer is a TW_IMAGEMEMXFER structure.
    ' *                    
    ' * Returns:
    ' *  ReturnCode
    ' *          Example: TWRC_SUCCESS.
    ' *
    ' * Note:
    ' *  The DSPROC type is only used by an application when it calls
    ' *  a Data Source directly, bypassing the Data Source Manager.
    ' *
    ' ********************************************************************/
    '/* Don't mangle the name "DS_Entry" if we're compiling in C++! */
    '#ifdef  __cplusplus
    'extern "C" {
    '#endif  /* __cplusplus */

    '#ifdef TWH_CMP_MSC

    '  TW_UINT16 FAR PASCAL DS_Entry(pTW_IDENTITY pOrigin,
    '                                TW_UINT32 DG, 
    '                                TW_UINT16 DAT, 
    '                                TW_UINT16 MSG,
    '                                TW_MEMREF pData);

    '  typedef TW_UINT16 (FAR PASCAL *DSENTRYPROC)(pTW_IDENTITY pOrigin,
    '                                              TW_UINT32 DG,
    '                                              TW_UINT16 DAT,
    '                                              TW_UINT16 MSG,
    '                                              TW_MEMREF pData);

    '#else

    '  FAR PASCAL TW_UINT16 DS_Entry(pTW_IDENTITY pOrigin,
    '                                TW_UINT32 DG,
    '                                TW_UINT16 DAT,
    '                                TW_UINT16 MSG,
    '                                TW_MEMREF pData);

    '  typedef TW_UINT16 (*DSENTRYPROC)(pTW_IDENTITY pOrigin,
    '                                   TW_UINT32 DG,
    '                                   TW_UINT16 DAT,
    '                                   TW_UINT16 MSG,
    '                                   TW_MEMREF pData);

    '#endif /* TWH_CMP_MSC */


    '  typedef TW_HANDLE (PASCAL *DSM_MEMALLOCATE)(TW_UINT32 _size);
    '  typedef void (PASCAL *DSM_MEMFREE)(TW_HANDLE _handle);
    '  typedef TW_MEMREF (PASCAL *DSM_MEMLOCK)(TW_HANDLE _handle);
    '  typedef void (PASCAL *DSM_MEMUNLOCK)(TW_HANDLE _handle);

    '#ifdef  __cplusplus
    '}
    '#endif  /* __cplusplus */

    '/* DAT_ENTRYPOINT. returns essential entry points. */
    'typedef struct {
    '   TW_UINT32         Size;
    '   DSMENTRYPROC      DSM_Entry;
    '   DSM_MEMALLOCATE   DSM_MemAllocate;
    '   DSM_MEMFREE       DSM_MemFree;
    '   DSM_MEMLOCK       DSM_MemLock;
    '   DSM_MEMUNLOCK     DSM_MemUnlock;
    '} TW_ENTRYPOINT, FAR * pTW_ENTRYPOINT;






    '<ClassInterface(ClassInterfaceType.AutoDual)> Public Class TWAIN_Function
    <StructLayout(LayoutKind.Sequential, Pack:=2, CharSet:=CharSet.Ansi)> _
    Public Structure TW_VERSION
        Dim MajorNum As UInt16      '/* Major revision number of the software. */
        Dim MinorNum As UInt16      '/* Incremental revision number of the software. */
        Dim Language As UInt16      '/* e.g. TWLG_SWISSFRENCH */
        Dim Country As UInt16       '/* e.g. TWCY_SWITZERLAND */
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=34)> _
        Dim Info As String '*32     '/* e.g. "1.0b3 Beta release" */  
    End Structure

    <StructLayout(LayoutKind.Sequential, Pack:=2, CharSet:=CharSet.Ansi)> _
    Public Structure TW_IDENTITY
        Dim Id As UInt32
        Dim Version As TW_VERSION
        Dim ProtocolMajor As UInt16
        Dim ProtocolMinor As UInt16
        Dim SupportedGroups As UInt32
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=34)> _
        Dim Manufacturer As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=34)> _
        Dim ProductFamily As String
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=34)> _
        Dim ProductName As String
    End Structure
    ' Imports System.Runtime.InteropServices


    Public Class DS_Entry_Pump
        'Public Structure TW_VERSION
        '    Dim MajorNum As UInt16      '/* Major revision number of the software. */
        '    Dim MinorNum As UInt16      '/* Incremental revision number of the software. */
        '    Dim Language As UInt16      '/* e.g. TWLG_SWISSFRENCH */
        '    Dim Country As UInt16       '/* e.g. TWCY_SWITZERLAND */
        '    Dim Info As String '*32     '/* e.g. "1.0b3 Beta release" */  
        'End Structure

        'Public Structure TW_IDENTITY
        '    Dim Id As UInt32
        '    Dim Version As TW_VERSION
        '    Dim ProtocolMajor As UInt16
        '    Dim ProtocolMinor As UInt16
        '    Dim SupportedGroups As UInt32
        '    Dim Manufacturer As String
        '    Dim ProductFamily As String
        '    Dim ProductName As String
        'End Structure
        Private Enum TwainState As Integer
            None = 0
            DSM_Pre_Session = 1
            DSM_Loaded = 2
            DSM_Opened = 3
            DS_Opened = 4
            DS_Enabled = 5
            DS_Xfer_Ready = 6
            DS_Xfer_Active = 7
        End Enum

        Public Enum RequestSource As Byte
            SANE = 0
            TWAIN = 1
        End Enum

        Private Structure TwainCapability
            Dim Capability As CAP
            Dim DataType As TWTY
            Dim EnumType As Type
            'Dim EnumObject As Object
            Dim DefaultValue As Object
            Dim CurrentValue As Object
            Dim SupportedOperations As TWQC
        End Structure

        Private Const TW_TRUE = 1
        Private Const TW_FALSE = 0
        Private MyTWAINversion As Single = 0

        Private MyIdentity As TW_IDENTITY
        'Private pMyIdentity As IntPtr
        Private AppIdentity As TW_IDENTITY
        'Private pAppIdentity As IntPtr
        Private MyState As TwainState
        Private MyCondition As TWCC
        Private MyResult As TWRC
        Private MyForm As FormMain
        Private Caps As New Dictionary(Of CAP, TwainCapability)
        Private Structure TWAINImage
            Dim DIB As DIB
            'Dim hBitmap As IntPtr
            Dim ImageInfo As TW_IMAGEINFO
            Dim ImageLayout As TW_IMAGELAYOUT
            Dim TotalBytes As UInt32
            Dim BytesTransferred As UInt32
            Dim LinesTransferred As UInt32
        End Structure

        Private Structure TWAINJob
            Dim PendingXfers As TW_PENDINGXFERS
            Dim CurrentImage As TWAINImage
            Dim ImagesXferred As Integer
        End Structure
        <Serializable()> _
        Public Structure CustomDSData
            Dim ScanContinuously As Boolean
            Dim ScanContinouslyUserConfigured As Boolean
            Dim SANEOptionValues() As Object
        End Structure
        Private CurrentJob As TWAINJob
        Public Delegate Sub Message_From_DSEventHandler(ByVal _pOrigin As IntPtr, ByVal _pDest As IntPtr, ByVal _DG As UInt32, ByVal _DAT As UInt32, ByVal _MSG As UInt16, ByVal _pData As IntPtr)
        Public Event Message_From_DS As Message_From_DSEventHandler
        'Private Logger As New Logger

        Private Sub Send_TWAIN_Message(ByVal _Origin As TW_IDENTITY, ByVal _Dest As TW_IDENTITY, ByVal _DG As DG, ByVal _DAT As DAT, ByVal _MSG As MSG, ByVal _Data As Object)

            Dim LogString As String = "(" & _DG.ToString & ", " & _DAT.ToString & ", " & _MSG.ToString & ")"
            LogString += " Origin.Id=" & _Origin.Id.ToString
            LogString += ", Origin.ProductName=" & _Origin.ProductName
            LogString += ", Dest.Id=" & _Dest.Id.ToString
            LogString += ", Dest.ProductName=" & _Dest.ProductName

            Logger.Write(DebugLogger.Level.Info, False, LogString)

            'XXX check for null ptr after each mem alloc!

            'Dim _pOrigin As IntPtr = Marshal.AllocHGlobal(Marshal.SizeOf(_Origin))
            'Dim _pDest As IntPtr = Marshal.AllocHGlobal(Marshal.SizeOf(_Dest))
            Dim _pOrigin As IntPtr = WinAPI.GlobalAlloc(WinAPI.GlobalAllocFlags.GHND, Marshal.SizeOf(_Origin))
            Dim _pDest As IntPtr = WinAPI.GlobalAlloc(WinAPI.GlobalAllocFlags.GHND, Marshal.SizeOf(_Dest))
            Dim _pData As IntPtr = IntPtr.Zero

            Marshal.StructureToPtr(_Origin, _pOrigin, True)
            Marshal.StructureToPtr(_Dest, _pDest, True)

            If _Data IsNot Nothing Then
                '_pData = Marshal.AllocHGlobal(Marshal.SizeOf(_Data))
                _pData = WinAPI.GlobalAlloc(WinAPI.GlobalAllocFlags.GHND, Marshal.SizeOf(_Data))
                Marshal.StructureToPtr(_Data, _pData, True)
            End If

            RaiseEvent Message_From_DS(_pOrigin, _pDest, _DG, _DAT, _MSG, _pData)

            'Per the TWAIN spec it seems to be the TWAIN Application's responsibility to free the memory (this point is unclear)

        End Sub

        Private Sub SetState(ByVal NewState As TwainState)

            Dim caller As String = Nothing
            Dim callertype As String = Nothing
            Try
                Dim stackTrace As StackTrace = New StackTrace
                caller = stackTrace.GetFrame(1).GetMethod.Name
                callertype = stackTrace.GetFrame(1).GetMethod.ReflectedType.Name
            Catch ex As Exception
            End Try

            Logger.Write(DebugLogger.Level.Debug, False, MyState.ToString & " --> " & NewState.ToString & "; called from " & "[" & callertype & ":" & caller & "]")
            MyState = NewState
        End Sub

        Private Sub SetCondition(ByVal NewCondition As TWCC)
            'If NewCondition <> MyCondition Then
            Logger.Write(DebugLogger.Level.Debug, False, MyCondition.ToString & " --> " & NewCondition.ToString)
            MyCondition = NewCondition
            'End If
        End Sub

        Private Sub SetResult(ByVal NewResult As TWRC)
            'If NewResult <> MyResult Then
            If (NewResult <> TWRC.TWRC_DSEVENT) And (NewResult <> TWRC.TWRC_NOTDSEVENT) Then 'filter out the noisy changes
                Logger.Write(DebugLogger.Level.Debug, False, MyResult.ToString & " --> " & NewResult.ToString)
            End If
            MyResult = NewResult
            'End If
        End Sub

        Public Sub Log(ByVal message As String)
            Logger.Write(DebugLogger.Level.Info, False, message)
        End Sub

        '        Public Function Message_To_DS(ByVal _Origin As TW_IDENTITY, ByVal _DG As UInt32, ByVal _DAT As UInt32, ByVal _MSG As UInt16, ByVal _pData As IntPtr) As Int16
        Public Function Message_To_DS(ByVal _pOrigin As IntPtr, ByVal _DG As UInt32, ByVal _DAT As UInt32, ByVal _MSG As UInt16, ByVal _pData As IntPtr) As Int16
            '/**********************************************************************
            ' * Function: DS_Entry, the entry point provided by a Data Source.
            ' *
            ' * Parameters:
            ' *  pOrigin Identifies the source module of the message. This could
            ' *          identify an application or the Data Source Manager.
            ' *
            ' *  DG      The Data Group. 
            ' *          Example: DG_IMAGE.
            ' *           
            ' *  DAT     The Data Attribute Type.
            ' *          Example: DAT_IMAGEMEMXFER.
            ' *    
            ' *  MSG     The message.  Messages are interpreted by the data source
            ' *          with respect to the Data Group and the Data Attribute Type.
            ' *          Example: MSG_GET.
            ' *
            ' *  pData   A pointer to the data structure or variable identified 
            ' *          by the Data Attribute Type.
            ' *          Example: (TW_MEMREF)&ImageMemXfer
            ' *                   where ImageMemXfer is a TW_IMAGEMEMXFER structure.
            ' *                    
            ' * Returns:
            ' *  ReturnCode
            ' *          Example: TWRC_SUCCESS.
            ' *
            ' * Note:
            ' *  The DSPROC type is only used by an application when it calls
            ' *  a Data Source directly, bypassing the Data Source Manager.
            ' *
            ' ********************************************************************/

            Dim DG As DG = _DG
            Dim DAT As DAT = _DAT
            Dim MSG As MSG = _MSG

            If Not ((_DG = DG.DG_CONTROL) And (_DAT = DAT.DAT_EVENT) And (_MSG = MSG.MSG_PROCESSEVENT)) Then
                Dim LogString As String = "(" & DG.ToString & ", " & DAT.ToString & ", " & MSG.ToString & ")"
                'If _pOrigin <> IntPtr.Zero Then
                '    Dim _Origin As TW_IDENTITY_unmanaged = Marshal.PtrToStructure(_pData, GetType(TW_IDENTITY_unmanaged))
                '    LogString += " Origin.Id=" & _Origin.Id.ToString
                '    LogString += ", Origin.ProductName=" & _Origin.ProductName
                '    LogString += ", Origin.Manufacturer=" & _Origin.Manufacturer
                'End If
                Logger.Write(DebugLogger.Level.Debug, False, LogString)
            End If

            If Not ((_DG = DG.DG_CONTROL) And (_DAT = DAT.DAT_STATUS) And (_MSG = MSG.MSG_GET)) Then
                If MyCondition <> TWCC.TWCC_SUCCESS Then Me.SetCondition(TWCC.TWCC_SUCCESS)
            End If

            Select Case DG
                'Case DG.DG_AUDIO
                Case DG.DG_CONTROL
                    Select Case DAT
                        Case DAT.DAT_CAPABILITY
                            Return DG_CONTROL__DAT_CAPABILITY(MSG, _pData)
                        Case DAT.DAT_EVENT
                            Return DG_CONTROL__DAT_EVENT(MSG, _pData)
                        Case DAT.DAT_IDENTITY
                            Return DG_CONTROL__DAT_IDENTITY(_pOrigin, MSG, _pData)
                        Case DAT.DAT_STATUS
                            Return DG_CONTROL__DAT_STATUS(MSG, _pData)
                        Case DAT.DAT_USERINTERFACE
                            Return DG_CONTROL__DAT_USERINTERFACE(MSG, _pData)
                        Case DAT.DAT_PENDINGXFERS
                            Return DG_CONTROL__DAT_PENDINGXFERS(MSG, _pData)
                        Case DAT.DAT_SETUPMEMXFER
                            Return DG_CONTROL__DAT_SETUPMEMXFER(MSG, _pData)
                        Case DAT.DAT_CUSTOMDSDATA
                            Return DG_CONTROL__DAT_CUSTOMDSDATA(MSG, _pData)
                        Case Else
                            'XXX
                    End Select
                Case DG.DG_IMAGE
                    Select Case DAT
                        Case DAT.DAT_IMAGEINFO
                            Return DG_IMAGE__DAT_IMAGEINFO(MSG, _pData)
                        Case DAT.DAT_IMAGELAYOUT
                            Return DG_IMAGE__DAT_IMAGELAYOUT(MSG, _pData)
                        Case DAT.DAT_IMAGENATIVEXFER
                            Return DG_IMAGE__DAT_IMAGENATIVEXFER(MSG, _pData)
                        Case DAT.DAT_IMAGEMEMXFER
                            Return DG_IMAGE__DAT_IMAGEMEMXFER(MSG, _pData)
                        Case Else
                            'XXX
                    End Select
                Case Else
                    SetCondition(TWCC.TWCC_BADPROTOCOL)
                    SetResult(TWRC.TWRC_FAILURE)
                    Return MyResult
            End Select

            SetCondition(TWCC.TWCC_BUMMER)
            SetResult(TWRC.TWRC_FAILURE)
            Return MyResult
        End Function

        Private Function DG_CONTROL__DAT_SETUPMEMXFER(ByVal _MSG As MSG, ByVal _pData As IntPtr) As TWRC
            Select Case _MSG
                Case MSG.MSG_GET
                    If (MyState >= TwainState.DS_Opened) And (MyState <= TwainState.DS_Xfer_Ready) Then
                        Dim SetupMem As TW_SETUPMEMXFER
                        'SetupMem.MinBufSize = TWON.TWON_DONTCARE32
                        'SetupMem.MaxBufSize = TWON.TWON_DONTCARE32
                        'SetupMem.Preferred = TWON.TWON_DONTCARE32
                        SetupMem.MinBufSize = 65536
                        SetupMem.MaxBufSize = 65536
                        SetupMem.Preferred = 65536
                        Marshal.StructureToPtr(SetupMem, _pData, True)
                        SetResult(TWRC.TWRC_SUCCESS)
                        Return MyResult
                    Else
                        SetCondition(TWCC.TWCC_SEQERROR)
                        SetResult(TWRC.TWRC_FAILURE)
                        Return MyResult
                    End If
                Case Else
                    SetCondition(TWCC.TWCC_BADPROTOCOL)
                    SetResult(TWRC.TWRC_FAILURE)
                    Return MyResult
            End Select
        End Function

        Private Function DG_IMAGE__DAT_IMAGELAYOUT(ByVal _MSG As MSG, ByVal _pData As IntPtr) As TWRC
            Select Case _MSG
                Case MSG.MSG_GET
                    If (MyState >= TwainState.DS_Opened) And (MyState <= TwainState.DS_Xfer_Ready) Then
                        With CurrentJob.CurrentImage.ImageLayout
                            .DocumentNumber = 1 'XXX
                            .PageNumber = 1 'XXX
                            .FrameNumber = 1 'XXX
                            .Frame.Top = FloatToFIX32(0.0)
                            .Frame.Left = FloatToFIX32(0.0)
                            .Frame.Bottom = FloatToFIX32(Caps(CAP.ICAP_PHYSICALHEIGHT).CurrentValue)
                            .Frame.Right = FloatToFIX32(Caps(CAP.ICAP_PHYSICALWIDTH).CurrentValue)
                        End With
                        Marshal.StructureToPtr(CurrentJob.CurrentImage.ImageLayout, _pData, True)
                        SetResult(TWRC.TWRC_SUCCESS)
                        Return MyResult
                    Else
                        SetCondition(TWCC.TWCC_SEQERROR)
                        SetResult(TWRC.TWRC_FAILURE)
                        Return MyResult
                    End If
                Case Else
                    SetCondition(TWCC.TWCC_BADPROTOCOL)
                    SetResult(TWRC.TWRC_FAILURE)
                    Return MyResult
            End Select
        End Function

        Private Function DG_CONTROL__DAT_PENDINGXFERS(ByVal _MSG As MSG, ByVal _pData As IntPtr) As TWRC
            Logger.Write(DebugLogger.Level.Debug, False, "CurrentJob.PendingXfers.Count = " & CurrentJob.PendingXfers.Count.ToString)
            Select Case _MSG
                Case MSG.MSG_GET
                    If MyState >= TwainState.DS_Opened And MyState <= TwainState.DS_Xfer_Active Then

                        Marshal.StructureToPtr(CurrentJob.PendingXfers, _pData, True)

                        SetResult(TWRC.TWRC_SUCCESS)
                        Return MyResult
                    Else
                        SetCondition(TWCC.TWCC_SEQERROR)
                        SetResult(TWRC.TWRC_FAILURE)
                        Return MyResult
                    End If
                Case MSG.MSG_ENDXFER
                    CurrentJob.CurrentImage = Nothing

                    SetState(TwainState.DS_Xfer_Ready)

                    If (Caps(CAP.CAP_FEEDERENABLED).CurrentValue <> 0) Then
                        SetPendingXfers(-1)
                    Else
                        Dim p As Integer = CurrentJob.PendingXfers.Count
                        p -= 1
                        If p < 0 Then p = 0
                        SetPendingXfers(p)
                    End If

                    If (Caps(CAP.CAP_XFERCOUNT).CurrentValue > -1) AndAlso (Caps(CAP.CAP_XFERCOUNT).CurrentValue <= CurrentJob.ImagesXferred) Then
                        If CurrentJob.PendingXfers.Count <> 0 Then SetPendingXfers(0)
                    End If

                    If CurrentJob.PendingXfers.Count = 0 Then
                        SetState(TwainState.DS_Enabled)
                        SetXfers(0)
                    End If
                    Marshal.StructureToPtr(CurrentJob.PendingXfers, _pData, True)
                    SetResult(TWRC.TWRC_SUCCESS)
                    Return MyResult
                Case MSG.MSG_RESET
                    If MyState = TwainState.DS_Xfer_Active Then SetState(TwainState.DS_Xfer_Ready) 'Spec says MSG_RESET is only valid in state 6, but Acrobat sends it in state 7.
                    If (MyState = TwainState.DS_Xfer_Ready) Or (MyState = TwainState.DS_Enabled) Then 'Spec says MSG_RESET is only valid in state 6, but Word 2003 sends it in state 5.
                        If MyState <> TwainState.DS_Enabled Then SetState(TwainState.DS_Enabled)
                        SetPendingXfers(0)
                        SetXfers(0)
                        Marshal.StructureToPtr(CurrentJob.PendingXfers, _pData, True)
                        SetResult(TWRC.TWRC_SUCCESS)
                        Return MyResult
                    Else
                        SetCondition(TWCC.TWCC_SEQERROR)
                        SetResult(TWRC.TWRC_FAILURE)
                        Return MyResult
                    End If
                Case Else
                    SetCondition(TWCC.TWCC_BADPROTOCOL)
                    SetResult(TWRC.TWRC_FAILURE)
                    Return MyResult
            End Select
        End Function

        Private Sub SetXfers(ByVal Count As Integer)
            Logger.Write(DebugLogger.Level.Debug, False, "CurrentJob.ImagesXferred " & CurrentJob.ImagesXferred.ToString & " --> " & Count.ToString)
            CurrentJob.ImagesXferred = Count
        End Sub

        Private Sub SetPendingXfers(ByVal Count As Integer)
            Logger.Write(DebugLogger.Level.Debug, False, "CurrentJob.PendingXfers.Count " & CurrentJob.PendingXfers.Count.ToString & " --> " & Count.ToString)
            CurrentJob.PendingXfers.Count = Count
        End Sub

        Private Function DG_IMAGE__DAT_IMAGENATIVEXFER(ByVal _MSG As MSG, ByVal _pData As IntPtr) As TWRC
            Select Case _MSG
                Case MSG.MSG_GET
                    Select Case MyState
                        Case TwainState.DS_Xfer_Ready
                            SetState(TwainState.DS_Xfer_Active)
                            Try
                                Dim Status As SANE_API.SANE_Status
                                Dim bmp As System.Drawing.Bitmap = Nothing
                                Status = AcquireImage(bmp)
                                If Status = SANE_API.SANE_Status.SANE_STATUS_GOOD Then
                                    If Caps(CAP.CAP_FEEDERENABLED).CurrentValue <> 0 Then
                                        'SetPendingXfers(-1)
                                    Else
                                        If Caps(CAP.CAP_DUPLEXENABLED).CurrentValue <> 0 Then
                                            If CurrentJob.ImagesXferred = 0 Then SetPendingXfers(2)
                                        Else
                                            SetPendingXfers(1)
                                        End If
                                    End If
                                    SetXfers(CurrentJob.ImagesXferred + 1)
                                    If bmp IsNot Nothing Then
                                        UpdateImageInfo(bmp)
                                        CurrentJob.CurrentImage.DIB = New DIB(bmp)
                                        bmp.Dispose()
                                        Marshal.StructureToPtr(CurrentJob.CurrentImage.DIB.CreateUnmanagedCopy, _pData, True)
                                        CurrentJob.CurrentImage.DIB = Nothing

                                        SetResult(TWRC.TWRC_XFERDONE)
                                        Return MyResult

                                    Else
                                        Throw New ApplicationException("AcquireImage() returned SANE_STATUS_GOOD but no bitmap!")
                                    End If
                                Else
                                    Select Case Status
                                        Case SANE_API.SANE_Status.SANE_STATUS_ACCESS_DENIED
                                            SetCondition(TWCC.TWCC_DENIED)
                                        Case SANE_API.SANE_Status.SANE_STATUS_NO_DOCS
                                            SetCondition(TWCC.TWCC_NOMEDIA)
                                        Case SANE_API.SANE_Status.SANE_STATUS_JAMMED
                                            SetCondition(TWCC.TWCC_PAPERJAM)
                                        Case SANE_API.SANE_Status.SANE_STATUS_NO_MEM
                                            SetCondition(TWCC.TWCC_LOWMEMORY)
                                        Case Else
                                            SetCondition(TWCC.TWCC_BUMMER)
                                    End Select
                                    SetPendingXfers(0)
                                    'SetResult(TWRC.TWRC_FAILURE)
                                    If MyCondition = TWCC.TWCC_NOMEDIA Then SetResult(TWRC.TWRC_CANCEL) Else SetResult(TWRC.TWRC_FAILURE) 'TWCC_NOMEDIA was introduced in 2.0.  I guess prior to that TWRC_CANCEL meant out of paper.
                                    Return MyResult
                                End If
                            Catch ex As Exception
                                Logger.Write(DebugLogger.Level.Error_, True, ex.Message)
                                SetCondition(TWCC.TWCC_OPERATIONERROR)
                                SetResult(TWRC.TWRC_FAILURE)
                                Return MyResult
                            End Try
                        Case Else
                            SetCondition(TWCC.TWCC_SEQERROR)
                            SetResult(TWRC.TWRC_FAILURE)
                            Return MyResult
                    End Select
                Case Else
                    SetCondition(TWCC.TWCC_BADPROTOCOL)
                    SetResult(TWRC.TWRC_FAILURE)
                    Return MyResult
            End Select
        End Function

        Private Function DG_IMAGE__DAT_IMAGEMEMXFER(ByVal _MSG As MSG, ByVal _pData As IntPtr) As TWRC
            Select Case _MSG
                Case MSG.MSG_GET
                    Dim ImageMemXfer As TW_IMAGEMEMXFER = Marshal.PtrToStructure(_pData, GetType(TW_IMAGEMEMXFER))
                    Select Case MyState
                        Case TwainState.DS_Xfer_Ready
                            Try
                                Dim Status As SANE_API.SANE_Status
                                Dim bmp As System.Drawing.Bitmap = Nothing
                                Status = AcquireImage(bmp)
                                If Status = SANE_API.SANE_Status.SANE_STATUS_GOOD Then

                                    If Caps(CAP.CAP_FEEDERENABLED).CurrentValue <> 0 Then
                                        'SetPendingXfers(-1)
                                    Else
                                        If Caps(CAP.CAP_DUPLEXENABLED).CurrentValue <> 0 Then
                                            If CurrentJob.ImagesXferred = 0 Then SetPendingXfers(2)
                                        Else
                                            SetPendingXfers(1)
                                        End If
                                    End If
                                    SetXfers(CurrentJob.ImagesXferred + 1)

                                    If bmp IsNot Nothing Then
                                        UpdateImageInfo(bmp)
                                        CurrentJob.CurrentImage.DIB = New DIB(bmp)
                                        bmp.Dispose()

                                        Dim bmiHeader As DIB.BITMAPINFOHEADER = CurrentJob.CurrentImage.DIB.DIBInfo.bmiHeader
                                        Dim AlignedBytesPerLine As UInt32 = ((bmiHeader.biWidth * bmiHeader.bitCount + 31) \ 32) * 4

                                        Logger.Write(DebugLogger.Level.Debug, False, "AlignedBytesPerLine=" & AlignedBytesPerLine.ToString)
                                        CurrentJob.CurrentImage.TotalBytes = AlignedBytesPerLine * CurrentJob.CurrentImage.ImageInfo.ImageLength
                                        Logger.Write(DebugLogger.Level.Debug, False, "CurrentImage.TotalBytes=" & CurrentJob.CurrentImage.TotalBytes.ToString)
                                        Logger.Write(DebugLogger.Level.Debug, False, "ImageMemXfer.Memory.Length=" & ImageMemXfer.Memory.Length.ToString)

                                        If ImageMemXfer.Memory.Length >= AlignedBytesPerLine Then
                                            Dim LinesToCopy As Integer = ImageMemXfer.Memory.Length \ AlignedBytesPerLine
                                            If LinesToCopy > CurrentJob.CurrentImage.ImageInfo.ImageLength Then LinesToCopy = CurrentJob.CurrentImage.ImageInfo.ImageLength
                                            CurrentJob.CurrentImage.DIB.CopyLinesToUnmanagedBuffer(0, LinesToCopy, AlignedBytesPerLine, ImageMemXfer.Memory.TheMem)
                                            CurrentJob.CurrentImage.LinesTransferred += LinesToCopy

                                            'XXX what if we transfer the whole image in this first buffer?



                                            'XXX this could be a lot more efficient
                                            'For i As UInteger = BytesToCopy To ImageMemXfer.Memory.Length - 1 'fill unused space with zeros
                                            '    'Logger.Write(DebugLogger.Level.Debug, False, "Zeroing one byte at position " & i.ToString)
                                            '    Marshal.WriteByte(ImageMemXfer.Memory.TheMem + i, 0)
                                            'Next

                                            'If Caps(CAP.CAP_FEEDERENABLED).CurrentValue <> 0 Then SetPendingXfers(-1) Else SetPendingXfers(0)

                                            SetState(TwainState.DS_Xfer_Active) 'if xfer fails on this first chunk we need to stay in state 6, so do this last.

                                            ImageMemXfer.BytesWritten = AlignedBytesPerLine * LinesToCopy
                                            ImageMemXfer.BytesPerRow = AlignedBytesPerLine
                                            ImageMemXfer.Rows = LinesToCopy
                                            ImageMemXfer.Columns = CurrentJob.CurrentImage.ImageInfo.ImageWidth
                                            ImageMemXfer.Compression = TWCP.TWCP_NONE
                                            ImageMemXfer.XOffset = 0
                                            'ImageMemXfer.YOffset = 0
                                            ImageMemXfer.YOffset = CurrentJob.CurrentImage.ImageInfo.ImageLength - LinesToCopy

                                            Marshal.StructureToPtr(ImageMemXfer, _pData, True)

                                            SetResult(TWRC.TWRC_SUCCESS)
                                            Return MyResult
                                        Else
                                            SetCondition(TWCC.TWCC_BADVALUE)
                                            SetResult(TWRC.TWRC_FAILURE)
                                            Return MyResult
                                        End If

                                    Else
                                        Throw New ApplicationException("AcquireImage() returned SANE_STATUS_GOOD but no bitmap!")
                                    End If
                                Else
                                    Select Case Status
                                        Case SANE_API.SANE_Status.SANE_STATUS_ACCESS_DENIED
                                            SetCondition(TWCC.TWCC_DENIED)
                                        Case SANE_API.SANE_Status.SANE_STATUS_NO_DOCS
                                            SetCondition(TWCC.TWCC_NOMEDIA)
                                        Case SANE_API.SANE_Status.SANE_STATUS_JAMMED
                                            SetCondition(TWCC.TWCC_PAPERJAM)
                                        Case SANE_API.SANE_Status.SANE_STATUS_NO_MEM
                                            SetCondition(TWCC.TWCC_LOWMEMORY)
                                        Case Else
                                            SetCondition(TWCC.TWCC_BUMMER)
                                    End Select
                                    SetPendingXfers(0)
                                    SetResult(TWRC.TWRC_FAILURE)
                                    Return MyResult
                                End If
                            Catch ex As Exception
                                Logger.Write(DebugLogger.Level.Error_, True, ex.Message)
                                SetCondition(TWCC.TWCC_OPERATIONERROR)
                                SetResult(TWRC.TWRC_FAILURE)
                                Return MyResult
                            End Try
                        Case TwainState.DS_Xfer_Active
                            Dim bmiHeader As DIB.BITMAPINFOHEADER = CurrentJob.CurrentImage.DIB.DIBInfo.bmiHeader
                            Dim AlignedBytesPerLine As UInt32 = ((bmiHeader.biWidth * bmiHeader.bitCount + 31) \ 32) * 4

                            If ImageMemXfer.Memory.Length >= AlignedBytesPerLine Then
                                Dim LinesToCopy As Integer = ImageMemXfer.Memory.Length \ AlignedBytesPerLine

                                If CurrentJob.CurrentImage.LinesTransferred + LinesToCopy > CurrentJob.CurrentImage.ImageInfo.ImageLength Then LinesToCopy = CurrentJob.CurrentImage.ImageInfo.ImageLength - CurrentJob.CurrentImage.LinesTransferred
                                CurrentJob.CurrentImage.DIB.CopyLinesToUnmanagedBuffer(CurrentJob.CurrentImage.LinesTransferred, LinesToCopy, AlignedBytesPerLine, ImageMemXfer.Memory.TheMem)
                                CurrentJob.CurrentImage.LinesTransferred += LinesToCopy

                                ImageMemXfer.BytesWritten = AlignedBytesPerLine * LinesToCopy
                                ImageMemXfer.BytesPerRow = AlignedBytesPerLine
                                ImageMemXfer.Rows = LinesToCopy
                                ImageMemXfer.Columns = CurrentJob.CurrentImage.ImageInfo.ImageWidth
                                ImageMemXfer.Compression = TWCP.TWCP_NONE
                                ImageMemXfer.XOffset = 0
                                'ImageMemXfer.YOffset = CurrentJob.CurrentImage.LinesTransferred
                                ImageMemXfer.YOffset = CurrentJob.CurrentImage.ImageInfo.ImageLength - CurrentJob.CurrentImage.LinesTransferred

                                Marshal.StructureToPtr(ImageMemXfer, _pData, True)

                                If CurrentJob.CurrentImage.LinesTransferred >= CurrentJob.CurrentImage.ImageInfo.ImageLength Then
                                    CurrentJob.CurrentImage.DIB = Nothing 'XXX need destructor here to free mem right away
                                    SetResult(TWRC.TWRC_XFERDONE)
                                Else
                                    SetResult(TWRC.TWRC_SUCCESS)
                                End If
                                Return MyResult
                            Else
                                SetCondition(TWCC.TWCC_BADVALUE)
                                SetResult(TWRC.TWRC_FAILURE)
                                Return MyResult
                            End If
                        Case Else
                            SetCondition(TWCC.TWCC_SEQERROR)
                            SetResult(TWRC.TWRC_FAILURE)
                            Return MyResult
                    End Select
                Case Else
                    SetCondition(TWCC.TWCC_BADPROTOCOL)
                    SetResult(TWRC.TWRC_FAILURE)
                    Return MyResult
            End Select
        End Function

        Private Sub UpdateImageInfo(ByRef bmp As System.Drawing.Bitmap)
            With CurrentJob.CurrentImage.ImageInfo
                ReDim .BitsPerSample(7)
                'XXX these values are in inches!  check ICAP_UNITS and handle.
                .XResolution = FloatToFIX32(bmp.HorizontalResolution)
                .YResolution = FloatToFIX32(bmp.VerticalResolution)
                .ImageWidth = bmp.Width
                .ImageLength = bmp.Height
                Select Case bmp.PixelFormat
                    Case Drawing.Imaging.PixelFormat.Format48bppRgb
                        .SamplesPerPixel = 3
                        .BitsPerSample(0) = 16
                        .BitsPerSample(1) = 16
                        .BitsPerSample(2) = 16
                        .BitsPerPixel = 48
                        .Planar = TW_FALSE
                        .PixelType = TWPT.TWPT_RGB
                    Case Drawing.Imaging.PixelFormat.Format24bppRgb
                        .SamplesPerPixel = 3
                        .BitsPerSample(0) = 8
                        .BitsPerSample(1) = 8
                        .BitsPerSample(2) = 8
                        .BitsPerPixel = 24
                        .Planar = TW_FALSE
                        .PixelType = TWPT.TWPT_RGB
                    Case Drawing.Imaging.PixelFormat.Format16bppGrayScale
                        .SamplesPerPixel = 1
                        .BitsPerSample(0) = 16
                        .BitsPerPixel = 16
                        .Planar = TW_TRUE
                        .PixelType = TWPT.TWPT_GRAY
                        SetCap(CAP.ICAP_PIXELFLAVOR, TWPF.TWPF_CHOCOLATE, RequestSource.TWAIN)
                    Case Drawing.Imaging.PixelFormat.Format8bppIndexed
                        .SamplesPerPixel = 1
                        .BitsPerSample(0) = 8
                        .BitsPerPixel = 8
                        .Planar = TW_TRUE
                        .PixelType = TWPT.TWPT_GRAY
                        SetCap(CAP.ICAP_PIXELFLAVOR, TWPF.TWPF_CHOCOLATE, RequestSource.TWAIN)
                    Case Drawing.Imaging.PixelFormat.Format1bppIndexed
                        .SamplesPerPixel = 1
                        .BitsPerSample(0) = 1
                        .BitsPerPixel = 1
                        .Planar = TW_TRUE
                        .PixelType = TWPT.TWPT_BW
                        SetCap(CAP.ICAP_PIXELFLAVOR, TWPF.TWPF_VANILLA, RequestSource.TWAIN)
                    Case Else
                        'XXX lots of formats to deal with here


                End Select
                .Compression = TWCP.TWCP_NONE

                Logger.Write(DebugLogger.Level.Debug, False, "XResolution -> " & FIX32ToFloat(.XResolution).ToString)
                Logger.Write(DebugLogger.Level.Debug, False, "YResolution -> " & FIX32ToFloat(.YResolution).ToString)
                Logger.Write(DebugLogger.Level.Debug, False, "ImageWidth -> " & .ImageWidth.ToString)
                Logger.Write(DebugLogger.Level.Debug, False, "ImageLength -> " & .ImageLength.ToString)
                Logger.Write(DebugLogger.Level.Debug, False, "SamplesPerPixel -> " & .SamplesPerPixel.ToString)
                For i As Integer = 0 To .BitsPerSample.Length - 1
                    Logger.Write(DebugLogger.Level.Debug, False, "BitsPerSample(" & i.ToString & ") -> " & .BitsPerSample(i).ToString)
                Next
                Logger.Write(DebugLogger.Level.Debug, False, "BitsPerPixel -> " & .BitsPerPixel.ToString)
                Logger.Write(DebugLogger.Level.Debug, False, "Planar -> " & CBool(.Planar).ToString)
                Logger.Write(DebugLogger.Level.Debug, False, "PixelType -> " & .PixelType.ToString)
                Logger.Write(DebugLogger.Level.Debug, False, "Compression -> " & .Compression.ToString)

            End With
        End Sub
        Private Sub UpdateImageInfo(ByVal SANEParams As SANE_API.SANE_Parameters)
            With CurrentJob.CurrentImage.ImageInfo
                ReDim .BitsPerSample(7)
                'XXX these values are in inches!  check ICAP_UNITS and handle.
                .XResolution = Caps(CAP.ICAP_XRESOLUTION).CurrentValue
                .YResolution = Caps(CAP.ICAP_YRESOLUTION).CurrentValue
                .ImageWidth = SANEParams.pixels_per_line
                .ImageLength = SANEParams.lines
                Select Case SANEParams.format
                    Case SANE_API.SANE_Frame.SANE_FRAME_RGB
                        Select Case SANEParams.depth
                            Case 16
                                .SamplesPerPixel = 3
                                .BitsPerSample(0) = 16
                                .BitsPerSample(1) = 16
                                .BitsPerSample(2) = 16
                                .BitsPerPixel = 48
                                .Planar = TW_FALSE
                                .PixelType = TWPT.TWPT_RGB
                            Case 8
                                .SamplesPerPixel = 3
                                .BitsPerSample(0) = 8
                                .BitsPerSample(1) = 8
                                .BitsPerSample(2) = 8
                                .BitsPerPixel = 24
                                .Planar = TW_FALSE
                                .PixelType = TWPT.TWPT_RGB
                            Case Else
                                'XXX
                        End Select
                    Case SANE_API.SANE_Frame.SANE_FRAME_GRAY
                        Select Case SANEParams.depth
                            Case 16
                                .SamplesPerPixel = 1
                                .BitsPerSample(0) = 16
                                .BitsPerPixel = 16
                                .Planar = TW_FALSE
                                .PixelType = TWPT.TWPT_GRAY
                                SetCap(CAP.ICAP_PIXELFLAVOR, TWPF.TWPF_CHOCOLATE, RequestSource.TWAIN)
                            Case 8
                                .SamplesPerPixel = 1
                                .BitsPerSample(0) = 8
                                .BitsPerPixel = 8
                                .Planar = TW_FALSE
                                .PixelType = TWPT.TWPT_GRAY
                                SetCap(CAP.ICAP_PIXELFLAVOR, TWPF.TWPF_CHOCOLATE, RequestSource.TWAIN)
                            Case 1
                                .SamplesPerPixel = 1
                                .BitsPerSample(0) = 1
                                .BitsPerPixel = 1
                                .Planar = TW_TRUE
                                .PixelType = TWPT.TWPT_BW
                                SetCap(CAP.ICAP_PIXELFLAVOR, TWPF.TWPF_VANILLA, RequestSource.TWAIN)
                            Case Else
                                'XXX
                        End Select
                    Case Else
                        'XXX 3 pass scanning.  Needs research.


                End Select
                .Compression = TWCP.TWCP_NONE

                Logger.Write(DebugLogger.Level.Debug, False, "XResolution -> " & FIX32ToFloat(.XResolution).ToString)
                Logger.Write(DebugLogger.Level.Debug, False, "YResolution -> " & FIX32ToFloat(.YResolution).ToString)
                Logger.Write(DebugLogger.Level.Debug, False, "ImageWidth -> " & .ImageWidth.ToString)
                Logger.Write(DebugLogger.Level.Debug, False, "ImageLength -> " & .ImageLength.ToString)
                Logger.Write(DebugLogger.Level.Debug, False, "SamplesPerPixel -> " & .SamplesPerPixel.ToString)
                For i As Integer = 0 To .BitsPerSample.Length - 1
                    Logger.Write(DebugLogger.Level.Debug, False, "BitsPerSample(" & i.ToString & ") -> " & .BitsPerSample(i).ToString)
                Next
                Logger.Write(DebugLogger.Level.Debug, False, "BitsPerPixel -> " & .BitsPerPixel.ToString)
                Logger.Write(DebugLogger.Level.Debug, False, "Planar -> " & CBool(.Planar).ToString)
                Logger.Write(DebugLogger.Level.Debug, False, "PixelType -> " & .PixelType.ToString)
                Logger.Write(DebugLogger.Level.Debug, False, "Compression -> " & .Compression.ToString)

            End With
        End Sub

        Private Function DG_IMAGE__DAT_IMAGEINFO(ByVal _MSG As MSG, ByVal _pData As IntPtr) As TWRC
            Select Case _MSG
                Case MSG.MSG_GET
                    Dim Status As SANE_API.SANE_Status
                    Dim bmp As System.Drawing.Bitmap = Nothing
                    Select Case MyState
                        Case TwainState.DS_Xfer_Active
                            'return ImageInfo already filled by DG_IMAGE__DAT_IMAGENATIVEXFER
                            Marshal.StructureToPtr(CurrentJob.CurrentImage.ImageInfo, _pData, True)
                            SetResult(TWRC.TWRC_SUCCESS)
                            Return MyResult
                        Case TwainState.DS_Xfer_Ready
                            Dim SANEparams As SANE_API.SANE_Parameters
                            Status = SANE.Net_Get_Parameters(net, SANE.CurrentDevice.Handle, SANEparams)
                            If Status = SANE_API.SANE_Status.SANE_STATUS_GOOD Then

                                UpdateImageInfo(SANEparams)

                                Marshal.StructureToPtr(CurrentJob.CurrentImage.ImageInfo, _pData, True)
                                SetResult(TWRC.TWRC_SUCCESS)
                                Return MyResult
                            Else
                                SetCondition(TWCC.TWCC_BUMMER)
                                SetResult(TWRC.TWRC_FAILURE)
                                Return MyResult
                            End If
                        Case Else
                            SetCondition(TWCC.TWCC_SEQERROR)
                            SetResult(TWRC.TWRC_FAILURE)
                            Return MyResult
                    End Select
                Case Else
                    SetCondition(TWCC.TWCC_BADPROTOCOL)
                    SetResult(TWRC.TWRC_FAILURE)
                    Return MyResult
            End Select
        End Function

        Private Function DG_CONTROL__DAT_STATUS(ByVal _MSG As MSG, ByVal _pData As IntPtr) As TWRC
            Select Case _MSG
                Case MSG.MSG_GET
                    Dim Status As TW_STATUS = Marshal.PtrToStructure(_pData, GetType(TW_STATUS))
                    Status.ConditionCode = MyCondition
                    Status.Data = 0
                    Marshal.StructureToPtr(Status, _pData, True)
                    Logger.Write(DebugLogger.Level.Debug, False, "Returning condition code '" & MyCondition.ToString & "'")
                    SetCondition(TWCC.TWCC_SUCCESS) 'must reset now per TWAIN spec.
                    SetResult(TWRC.TWRC_SUCCESS)
                    Return MyResult
                Case Else
                    SetCondition(TWCC.TWCC_BADPROTOCOL)
                    SetResult(TWRC.TWRC_FAILURE)
                    Return MyResult
            End Select
        End Function

        Private Function DG_CONTROL__DAT_CUSTOMDSDATA(ByVal _MSG As MSG, ByVal _pData As IntPtr) As TWRC
            Select Case _MSG
                Case MSG.MSG_GET
                    Try
                        Dim CustomDSData As TW_CUSTOMDSDATA
                        Dim cds As CustomDSData
                        cds.ScanContinuously = CurrentSettings.ScanContinuously
                        cds.ScanContinouslyUserConfigured = CurrentSettings.ScanContinuouslyUserConfigured
                        cds.SANEOptionValues = SANE.CurrentDevice.OptionValues

                        Dim ser As New System.Web.Script.Serialization.JavaScriptSerializer
                        Dim MyStr As String = ser.Serialize(cds)

                        Logger.Write(DebugLogger.Level.Debug, False, MyStr)

                        Dim b() As Byte
                        b = System.Text.Encoding.ASCII.GetBytes(MyStr)
                        CustomDSData.InfoLength = b.Length
                        Logger.Write(DebugLogger.Level.Debug, False, "InfoLength=" & CustomDSData.InfoLength.ToString)

                        CustomDSData.hData = WinAPI.GlobalAlloc(WinAPI.GlobalAllocFlags.GHND, b.Length)
                        Marshal.Copy(b, 0, CustomDSData.hData, b.Length)
                        Array.Resize(b, 0)

                        Marshal.StructureToPtr(CustomDSData, _pData, True)

                        SetResult(TWRC.TWRC_SUCCESS)
                        Return MyResult
                    Catch ex As Exception
                        Logger.Write(DebugLogger.Level.Error_, False, ex.Message)
                        Logger.Write(DebugLogger.Level.Error_, False, ex.InnerException.Message)
                        SetCondition(TWCC.TWCC_BUMMER)
                        SetResult(TWRC.TWRC_FAILURE)
                        Return MyResult
                    End Try
                Case MSG.MSG_SET
                    Try
                        Logger.Write(DebugLogger.Level.Debug, False, "_pData=" & _pData.ToString)
                        Dim CustomDSData As TW_CUSTOMDSDATA = Marshal.PtrToStructure(_pData, GetType(TW_CUSTOMDSDATA))
                        Logger.Write(DebugLogger.Level.Debug, False, "InfoLength=" & CustomDSData.InfoLength.ToString)
                        Logger.Write(DebugLogger.Level.Debug, False, "hData=" & CustomDSData.hData.ToString)

                        If (CustomDSData.InfoLength > 0) And (CustomDSData.hData <> IntPtr.Zero) Then
                            Dim b(CustomDSData.InfoLength - 1) As Byte
                            Marshal.Copy(CustomDSData.hData, b, 0, b.Length)

                            Dim MyStr As String = System.Text.Encoding.ASCII.GetString(b, 0, b.Length)

                            Logger.Write(DebugLogger.Level.Debug, False, MyStr)

                            Dim ser As New System.Web.Script.Serialization.JavaScriptSerializer
                            Dim cds As CustomDSData = ser.Deserialize(Of CustomDSData)(MyStr)

                            Dim StoredOptionValues() As Object = cds.SANEOptionValues
                            For Index As Integer = 0 To StoredOptionValues.Length - 1
                                Dim od As SANE_API.SANE_Option_Descriptor = SANE.CurrentDevice.OptionDescriptors(Index)
                                Dim OptionDifferent As Boolean = False
                                If od.type <> SANE_API.SANE_Value_Type.SANE_TYPE_GROUP Then
                                    If StoredOptionValues(Index) IsNot Nothing AndAlso SANE.CurrentDevice.OptionValues(Index) IsNot Nothing Then
                                        For val_idx As Integer = 0 To StoredOptionValues(Index).Length - 1
                                            If StoredOptionValues(Index)(val_idx) IsNot Nothing AndAlso SANE.CurrentDevice.OptionValues(Index)(val_idx) IsNot Nothing Then
                                                Logger.Write(DebugLogger.Level.Debug, False, "Option '" & od.name & "': Current = '" & SANE.CurrentDevice.OptionValues(Index)(val_idx).ToString & "', Stored = '" & StoredOptionValues(Index)(val_idx).ToString & "'")
                                                If StoredOptionValues(Index)(val_idx) <> SANE.CurrentDevice.OptionValues(Index)(val_idx) Then
                                                    OptionDifferent = True
                                                    Exit For
                                                End If
                                            Else
                                                Logger.Write(DebugLogger.Level.Debug, False, "Option '" & od.name & "' value(" & val_idx.ToString & ") is Nothing")
                                            End If
                                        Next
                                    Else
                                        Logger.Write(DebugLogger.Level.Debug, False, "Option '" & od.name & "' value is Nothing")
                                    End If
                                    Logger.Write(DebugLogger.Level.Debug, False, "Stored option '" & od.title & "' is " & IIf(OptionDifferent, "", "not ") & "different from current settings.")

                                    If OptionDifferent Then
                                        If Not MyForm.SetSANEOption(od.name, StoredOptionValues(Index)) Then Logger.Write(DebugLogger.Level.Warn, False, "Error setting '" & od.type.ToString & "' option '" & od.title & "'")
                                    End If
                                End If
                            Next

                            MyForm.PanelOpt.Controls.Clear()
                            'MyForm.GetOpts(False)

                            CurrentSettings.ScanContinuouslyUserConfigured = cds.ScanContinouslyUserConfigured
                            CurrentSettings.ScanContinuously = cds.ScanContinuously
                            MyForm.CheckBoxBatchMode.Checked = CurrentSettings.ScanContinuously

                            Array.Resize(b, 0)
                        Else
                            Logger.Write(DebugLogger.Level.Warn, False, "Custom DS Data structure wasn't provided by the application!")
                        End If

                        SetResult(TWRC.TWRC_SUCCESS)
                        Return MyResult
                    Catch ex As Exception
                        Logger.Write(DebugLogger.Level.Error_, False, ex.Message)
                        Logger.Write(DebugLogger.Level.Error_, False, ex.InnerException.Message)
                        SetCondition(TWCC.TWCC_BUMMER)
                        SetResult(TWRC.TWRC_FAILURE)
                        Return MyResult
                    End Try
                Case Else
                    SetCondition(TWCC.TWCC_BADPROTOCOL)
                    SetResult(TWRC.TWRC_FAILURE)
                    Return MyResult
            End Select
        End Function

        Private Function DG_CONTROL__DAT_CAPABILITY(ByVal _MSG As MSG, ByVal _pData As IntPtr) As TWRC
            Select Case _MSG
                Case MSG.MSG_GET, MSG.MSG_GETCURRENT, MSG.MSG_GETDEFAULT
                    Dim tw_cap As TW_CAPABILITY = Marshal.PtrToStructure(_pData, GetType(TW_CAPABILITY))
                    Logger.Write(DebugLogger.Level.Debug, False, "Capability=" & CType(tw_cap.Cap, CAP).ToString & ", ContainerType=" & CType(tw_cap.ConType, TWON).ToString)

                    Dim ReqCap As TwainCapability
                    Dim CurVal As Object = Nothing
                    Dim DefaultVal As Object = Nothing
                    Try
                        ReqCap = Caps(tw_cap.Cap)
                    Catch ex As Exception
                        SetCondition(TWCC.TWCC_CAPUNSUPPORTED)
                        SetResult(TWRC.TWRC_FAILURE)
                        Return MyResult
                    End Try
                    CurVal = ReqCap.CurrentValue
                    Logger.Write(DebugLogger.Level.Debug, False, "Current Value Type=" & CurVal.GetType.ToString)
                    If CurVal.GetType Is GetType(TW_FIX32) Then
                        Logger.Write(DebugLogger.Level.Debug, False, "Current Value=" & FIX32ToFloat(CurVal).ToString)
                    Else
                        Logger.Write(DebugLogger.Level.Debug, False, "Current Value=" & CurVal.ToString)
                    End If
                    DefaultVal = ReqCap.DefaultValue

                    Select Case tw_cap.Cap
                        Case CAP.CAP_AUTOFEED, CAP.CAP_ENABLEDSUIONLY, CAP.CAP_UICONTROLLABLE, CAP.CAP_DUPLEXENABLED, CAP.CAP_FEEDERENABLED, CAP.CAP_FEEDERLOADED, CAP.CAP_PAPERDETECTABLE
                            If (tw_cap.Cap = CAP.CAP_AUTOFEED) AndAlso (Caps(CAP.CAP_FEEDERENABLED).CurrentValue = TW_FALSE) Then
                                SetCondition(TWCC.TWCC_CAPUNSUPPORTED)
                                SetResult(TWRC.TWRC_FAILURE)
                                Return MyResult
                            Else
                                Dim oneval As TW_ONEVALUE
                                oneval.ItemType = TWTY.TWTY_BOOL
                                oneval.Item = IIf(_MSG = MSG.MSG_GETDEFAULT, DefaultVal, CurVal)
                                Logger.Write(DebugLogger.Level.Debug, False, "Returning value '" & oneval.Item.ToString & "'")
                                Dim pContainer As IntPtr = WinAPI.GlobalAlloc(WinAPI.GlobalAllocFlags.GHND, Marshal.SizeOf(oneval))
                                tw_cap.ConType = TWON.TWON_ONEVALUE
                                tw_cap.hContainer = pContainer
                                Marshal.StructureToPtr(oneval, pContainer, True)
                                Marshal.StructureToPtr(tw_cap, _pData, True)
                                SetResult(TWRC.TWRC_SUCCESS)
                                Return MyResult
                            End If

                        Case CAP.CAP_SUPPORTEDCAPS

                            'XXX this is all very sucky
                            Dim cap_array As TW_ARRAY
                            cap_array.ItemType = TWTY.TWTY_UINT16
                            cap_array.NumItems = Caps.Count
                            Dim ItemOffset As Integer = Marshal.SizeOf(cap_array.ItemType) + Marshal.SizeOf(cap_array.NumItems)
                            Dim pContainer As IntPtr = WinAPI.GlobalAlloc(WinAPI.GlobalAllocFlags.GHND, CInt(ItemOffset + (2 * cap_array.NumItems)))
                            Marshal.WriteInt16(pContainer, cap_array.ItemType)
                            Marshal.WriteInt32(pContainer + 2, cap_array.NumItems)
                            Dim i As Integer = 0
                            For Each ccap As Object In Caps
                                Logger.Write(DebugLogger.Level.Debug, False, "  Supported capability: " & CType(ccap.Key, CAP).ToString)
                                Try
                                    Marshal.WriteInt16(pContainer + ItemOffset + i, ccap.Key)
                                Catch ex As Exception
                                    Logger.Write(DebugLogger.Level.Error_, True, ex.Message)
                                End Try
                                i += 2
                            Next
                            '

                            tw_cap.ConType = TWON.TWON_ARRAY
                            tw_cap.hContainer = pContainer
                            Marshal.StructureToPtr(tw_cap, _pData, True)
                            SetResult(TWRC.TWRC_SUCCESS)
                            Return MyResult

                        Case CAP.CAP_XFERCOUNT
                            Dim oneval As TW_ONEVALUE
                            oneval.ItemType = TWTY.TWTY_INT16
                            oneval.Item = IIf(_MSG = MSG.MSG_GETDEFAULT, DefaultVal, CurVal)
                            Dim pContainer As IntPtr = WinAPI.GlobalAlloc(WinAPI.GlobalAllocFlags.GHND, Marshal.SizeOf(oneval))
                            tw_cap.ConType = TWON.TWON_ONEVALUE
                            tw_cap.hContainer = pContainer
                            Marshal.StructureToPtr(oneval, pContainer, True)
                            Marshal.StructureToPtr(tw_cap, _pData, True)
                            SetResult(TWRC.TWRC_SUCCESS)
                            Return MyResult

                        Case CAP.ICAP_PIXELFLAVOR, CAP.ICAP_PIXELTYPE, CAP.ICAP_PLANARCHUNKY, CAP.ICAP_BITDEPTH, CAP.ICAP_UNITS, CAP.ICAP_XFERMECH, CAP.ICAP_BITORDER, CAP.ICAP_COMPRESSION, CAP.CAP_DUPLEX
                            Dim oneval As TW_ONEVALUE
                            oneval.ItemType = TWTY.TWTY_UINT16
                            oneval.Item = IIf(_MSG = MSG.MSG_GETDEFAULT, DefaultVal, CurVal)
                            Dim pContainer As IntPtr = WinAPI.GlobalAlloc(WinAPI.GlobalAllocFlags.GHND, Marshal.SizeOf(oneval))
                            tw_cap.ConType = TWON.TWON_ONEVALUE
                            tw_cap.hContainer = pContainer
                            Marshal.StructureToPtr(oneval, pContainer, True)
                            Marshal.StructureToPtr(tw_cap, _pData, True)
                            SetResult(TWRC.TWRC_SUCCESS)
                            Return MyResult

                        Case CAP.ICAP_PHYSICALHEIGHT, CAP.ICAP_PHYSICALWIDTH, CAP.ICAP_XRESOLUTION, CAP.ICAP_YRESOLUTION
                            'XXX Need to return a TW_ENUMERATION for resolution values

                            Select Case Caps(CAP.ICAP_UNITS).CurrentValue
                                Case TWUN.TWUN_INCHES, TWUN.TWUN_MILLIMETERS, TWUN.TWUN_CENTIMETERS
                                    Dim oneval As TW_ONEVALUE_FIX32
                                    oneval.ItemType = TWTY.TWTY_FIX32
                                    Select Case Caps(CAP.ICAP_UNITS).CurrentValue
                                        Case TWUN.TWUN_INCHES
                                            oneval.Item = IIf(_MSG = MSG.MSG_GETDEFAULT, DefaultVal, CurVal)
                                        Case TWUN.TWUN_MILLIMETERS
                                            oneval.Item = IIf(_MSG = MSG.MSG_GETDEFAULT, DefaultVal, FloatToFIX32(InchesToMM(FIX32ToFloat(CurVal))))
                                        Case TWUN.TWUN_CENTIMETERS
                                            oneval.Item = IIf(_MSG = MSG.MSG_GETDEFAULT, DefaultVal, FloatToFIX32(InchesToCM(FIX32ToFloat(CurVal))))
                                    End Select
                                    Dim pContainer As IntPtr = WinAPI.GlobalAlloc(WinAPI.GlobalAllocFlags.GHND, Marshal.SizeOf(oneval))
                                    tw_cap.ConType = TWON.TWON_ONEVALUE
                                    tw_cap.hContainer = pContainer
                                    Marshal.StructureToPtr(oneval, pContainer, True)
                                    Marshal.StructureToPtr(tw_cap, _pData, True)
                                    SetResult(TWRC.TWRC_SUCCESS)
                                Case Else
                                    Logger.Write(DebugLogger.Level.Warn, True, "unable to convert to unit '" & CType(Caps(CAP.ICAP_UNITS).CurrentValue, TWUN) & "'")
                                    SetCondition(TWCC.TWCC_BADVALUE)
                                    SetResult(TWRC.TWRC_FAILURE)
                            End Select
                            Return MyResult

                        Case Else
                            'XXX lots more CAPs to add here!

                            SetCondition(TWCC.TWCC_CAPUNSUPPORTED)
                            SetResult(TWRC.TWRC_FAILURE)
                            Return MyResult
                    End Select
                Case MSG.MSG_SET
                    If MyState <> TwainState.DS_Opened Then 'XXX we could negotiate to set caps in other states with CAP_EXTENDEDCAPS
                        SetCondition(TWCC.TWCC_SEQERROR)
                        SetResult(TWRC.TWRC_FAILURE)
                        Return MyResult
                    End If
                    Dim tw_cap As TW_CAPABILITY = Marshal.PtrToStructure(_pData, GetType(TW_CAPABILITY))
                    Logger.Write(DebugLogger.Level.Debug, False, "Capability=" & CType(tw_cap.Cap, CAP).ToString & ", ContainerType=" & CType(tw_cap.ConType, TWON).ToString)

                    Dim ReqCap As TwainCapability
                    Try
                        ReqCap = Caps(tw_cap.Cap)
                    Catch ex As Exception
                        SetCondition(TWCC.TWCC_CAPUNSUPPORTED)
                        SetResult(TWRC.TWRC_FAILURE)
                        Return MyResult
                    End Try

                    'XXX validate constraints & data types here

                    Select Case tw_cap.Cap
                        Case CAP.CAP_ENABLEDSUIONLY, CAP.CAP_UICONTROLLABLE, CAP.CAP_SUPPORTEDCAPS, CAP.CAP_FEEDERLOADED, CAP.CAP_DUPLEX 'ReadOnly caps
                            SetCondition(TWCC.TWCC_CAPBADOPERATION)
                            SetResult(TWRC.TWRC_FAILURE)
                            Return MyResult
                        Case CAP.CAP_AUTOFEED
                            If tw_cap.ConType <> TWON.TWON_ONEVALUE Then
                                SetCondition(TWCC.TWCC_BADVALUE)
                                SetResult(TWRC.TWRC_FAILURE)
                                Return MyResult
                            Else
                                Dim pContainer As IntPtr = WinAPI.GlobalLock(tw_cap.hContainer)
                                Dim oneval As TW_ONEVALUE = Marshal.PtrToStructure(pContainer, GetType(TW_ONEVALUE))
                                If pContainer Then WinAPI.GlobalUnlock(tw_cap.hContainer)
                                Logger.Write(DebugLogger.Level.Debug, False, "ItemType=" & CType(oneval.ItemType, TWTY).ToString)
                                If oneval.ItemType <> TWTY.TWTY_BOOL Then
                                    Me.SetCondition(TWCC.TWCC_BADVALUE)
                                    Me.SetResult(TWRC.TWRC_FAILURE)
                                    Return MyResult
                                End If

                                ''XXX should set a property rather than a checkbox
                                'MyForm.CheckBoxBatchMode.Checked = CBool(oneval.Item)
                                'MyForm.CheckBoxBatchMode.Enabled = False

                                Logger.Write(DebugLogger.Level.Debug, False, "app sent value '" & oneval.Item.ToString & "'")

                                SetCap(ReqCap.Capability, CType(oneval.Item, UInt16), RequestSource.TWAIN)

                                SetResult(TWRC.TWRC_SUCCESS)
                                Return MyResult
                            End If

                        Case CAP.CAP_DUPLEXENABLED
                            If tw_cap.ConType <> TWON.TWON_ONEVALUE Then
                                SetCondition(TWCC.TWCC_BADVALUE)
                                SetResult(TWRC.TWRC_FAILURE)
                                Return MyResult
                            Else
                                Dim pContainer As IntPtr = WinAPI.GlobalLock(tw_cap.hContainer)
                                Dim oneval As TW_ONEVALUE = Marshal.PtrToStructure(pContainer, GetType(TW_ONEVALUE))
                                If pContainer Then WinAPI.GlobalUnlock(tw_cap.hContainer)
                                Logger.Write(DebugLogger.Level.Debug, False, "ItemType=" & CType(oneval.ItemType, TWTY).ToString)

                                Logger.Write(DebugLogger.Level.Debug, False, "app sent value '" & oneval.Item.ToString & "'")

                                If oneval.ItemType <> TWTY.TWTY_BOOL Then
                                    Me.SetCondition(TWCC.TWCC_BADVALUE)
                                    Me.SetResult(TWRC.TWRC_FAILURE)
                                    Return MyResult
                                End If

                                SetCap(ReqCap.Capability, CType(oneval.Item, UInt16), RequestSource.TWAIN)
                                SetResult(TWRC.TWRC_SUCCESS)
                                Return MyResult
                            End If


                        Case CAP.CAP_FEEDERENABLED
                            If tw_cap.ConType <> TWON.TWON_ONEVALUE Then
                                SetCondition(TWCC.TWCC_BADVALUE)
                                SetResult(TWRC.TWRC_FAILURE)
                                Return MyResult
                            Else
                                Dim pContainer As IntPtr = WinAPI.GlobalLock(tw_cap.hContainer)
                                Dim oneval As TW_ONEVALUE = Marshal.PtrToStructure(pContainer, GetType(TW_ONEVALUE))
                                If pContainer Then WinAPI.GlobalUnlock(tw_cap.hContainer)
                                Logger.Write(DebugLogger.Level.Debug, False, "ItemType=" & CType(oneval.ItemType, TWTY).ToString)
                                If oneval.ItemType <> TWTY.TWTY_BOOL Then
                                    Me.SetCondition(TWCC.TWCC_BADVALUE)
                                    Me.SetResult(TWRC.TWRC_FAILURE)
                                    Return MyResult
                                End If

                                Logger.Write(DebugLogger.Level.Debug, False, "app sent value '" & oneval.Item.ToString & "'")

                                SetCap(ReqCap.Capability, CType(oneval.Item, UInt16), RequestSource.TWAIN)

                                CurrentSettings.ScanContinuously = CBool(Caps(CAP.CAP_FEEDERENABLED).CurrentValue)
                                MyForm.CheckBoxBatchMode.Checked = CurrentSettings.ScanContinuously 'XXX this may cause the cap to be set again through the CheckedChanged event
                                MyForm.CheckBoxBatchMode.Enabled = False 'if the TWAIN app is controlling this, don't let the user mess with it.

                                SetResult(TWRC.TWRC_SUCCESS)
                                Return MyResult
                            End If
                        Case CAP.CAP_XFERCOUNT
                            If tw_cap.ConType <> TWON.TWON_ONEVALUE Then
                                SetCondition(TWCC.TWCC_BADVALUE)
                                SetResult(TWRC.TWRC_FAILURE)
                                Return MyResult
                            Else
                                Dim pContainer As IntPtr = WinAPI.GlobalLock(tw_cap.hContainer)
                                Dim oneval As TW_ONEVALUE = Marshal.PtrToStructure(pContainer, GetType(TW_ONEVALUE))
                                If pContainer Then WinAPI.GlobalUnlock(tw_cap.hContainer)
                                Logger.Write(DebugLogger.Level.Debug, False, "ItemType=" & CType(oneval.ItemType, TWTY).ToString)
                                If oneval.ItemType <> TWTY.TWTY_INT16 Then
                                    Me.SetCondition(TWCC.TWCC_BADVALUE)
                                    Me.SetResult(TWRC.TWRC_FAILURE)
                                    Return MyResult
                                End If
                                SetCap(ReqCap.Capability, CType(oneval.Item, Int16), RequestSource.TWAIN)
                                SetResult(TWRC.TWRC_SUCCESS)
                                Return MyResult
                            End If

                        Case CAP.ICAP_BITDEPTH
                            If tw_cap.ConType <> TWON.TWON_ONEVALUE Then
                                SetCondition(TWCC.TWCC_BADVALUE)
                                SetResult(TWRC.TWRC_FAILURE)
                                Return MyResult
                            Else
                                Dim pContainer As IntPtr = WinAPI.GlobalLock(tw_cap.hContainer)
                                Dim oneval As TW_ONEVALUE = Marshal.PtrToStructure(pContainer, GetType(TW_ONEVALUE))
                                If pContainer Then WinAPI.GlobalUnlock(tw_cap.hContainer)
                                Logger.Write(DebugLogger.Level.Debug, False, "ItemType=" & CType(oneval.ItemType, TWTY).ToString)
                                If oneval.ItemType <> TWTY.TWTY_UINT16 Then
                                    SetCondition(TWCC.TWCC_BADVALUE)
                                    SetResult(TWRC.TWRC_FAILURE)
                                    Return MyResult
                                Else
                                    SetCap(ReqCap.Capability, CType(oneval.Item, UInt16), RequestSource.TWAIN)
                                    SetResult(TWRC.TWRC_SUCCESS)
                                    Return MyResult
                                End If
                            End If

                            'Case CAP.ICAP_BITORDER
                            '    'XXX
                            'Case CAP.ICAP_COMPRESSION
                            '    'XXX
                            'Case CAP.ICAP_PLANARCHUNKY
                            '    'XXX
                            'Case CAP.ICAP_PIXELFLAVOR
                            '    'XXX
                        Case CAP.ICAP_PIXELTYPE
                            If tw_cap.ConType <> TWON.TWON_ONEVALUE Then
                                SetCondition(TWCC.TWCC_BADVALUE)
                                SetResult(TWRC.TWRC_FAILURE)
                                Return MyResult
                            Else
                                Dim pContainer As IntPtr = WinAPI.GlobalLock(tw_cap.hContainer)
                                Dim oneval As TW_ONEVALUE = Marshal.PtrToStructure(pContainer, GetType(TW_ONEVALUE))
                                If pContainer Then WinAPI.GlobalUnlock(tw_cap.hContainer)
                                Logger.Write(DebugLogger.Level.Debug, False, "ItemType=" & CType(oneval.ItemType, TWTY).ToString)
                                If oneval.ItemType <> TWTY.TWTY_UINT16 Then
                                    SetCondition(TWCC.TWCC_BADVALUE)
                                    SetResult(TWRC.TWRC_FAILURE)
                                    Return MyResult
                                Else
                                    Select Case oneval.Item
                                        Case TWPT.TWPT_BW, TWPT.TWPT_GRAY, TWPT.TWPT_RGB
                                            SetCap(ReqCap.Capability, CType(oneval.Item, TWPT), RequestSource.TWAIN)
                                            SetResult(TWRC.TWRC_SUCCESS)
                                        Case Else
                                            SetCondition(TWCC.TWCC_BADVALUE)
                                            SetResult(TWRC.TWRC_FAILURE)
                                    End Select
                                    Return MyResult
                                End If
                            End If

                        Case CAP.ICAP_UNITS
                            If tw_cap.ConType <> TWON.TWON_ONEVALUE Then
                                SetCondition(TWCC.TWCC_BADVALUE)
                                SetResult(TWRC.TWRC_FAILURE)
                                Return MyResult
                            Else
                                Dim pContainer As IntPtr = WinAPI.GlobalLock(tw_cap.hContainer)
                                Dim oneval As TW_ONEVALUE = Marshal.PtrToStructure(pContainer, GetType(TW_ONEVALUE))
                                If pContainer Then WinAPI.GlobalUnlock(tw_cap.hContainer)
                                Logger.Write(DebugLogger.Level.Debug, False, "ItemType=" & CType(oneval.ItemType, TWTY).ToString)
                                If (oneval.ItemType <> TWTY.TWTY_UINT16) OrElse ((oneval.Item <> TWUN.TWUN_INCHES)) Then 'only allow inches, or...'And (oneval.Item <> TWUN.TWUN_MILLIMETERS) And (oneval.Item <> TWUN.TWUN_CENTIMETERS)) Then 'only allow inches, mm, or cm
                                    Me.SetCondition(TWCC.TWCC_BADVALUE)
                                    Me.SetResult(TWRC.TWRC_FAILURE)
                                    Return MyResult
                                End If
                                SetCap(ReqCap.Capability, CType(oneval.Item, TWUN), RequestSource.TWAIN)
                                SetResult(TWRC.TWRC_SUCCESS)
                                Return MyResult
                            End If

                        Case CAP.ICAP_XFERMECH
                            If tw_cap.ConType <> TWON.TWON_ONEVALUE Then
                                SetCondition(TWCC.TWCC_BADVALUE)
                                SetResult(TWRC.TWRC_FAILURE)
                                Return MyResult
                            Else
                                Dim pContainer As IntPtr = WinAPI.GlobalLock(tw_cap.hContainer)
                                Dim oneval As TW_ONEVALUE = Marshal.PtrToStructure(pContainer, GetType(TW_ONEVALUE))
                                If pContainer Then WinAPI.GlobalUnlock(tw_cap.hContainer)
                                Logger.Write(DebugLogger.Level.Debug, False, "ItemType=" & CType(oneval.ItemType, TWTY).ToString)
                                If oneval.ItemType <> TWTY.TWTY_UINT16 Then
                                    SetCondition(TWCC.TWCC_BADVALUE)
                                    SetResult(TWRC.TWRC_FAILURE)
                                    Return MyResult
                                Else
                                    Select Case oneval.Item
                                        Case TWSX.TWSX_NATIVE, TWSX.TWSX_MEMORY
                                            SetCap(ReqCap.Capability, CType(oneval.Item, TWSX), RequestSource.TWAIN)
                                            SetResult(TWRC.TWRC_SUCCESS)
                                        Case Else
                                            SetCondition(TWCC.TWCC_BADVALUE)
                                            SetResult(TWRC.TWRC_FAILURE)
                                    End Select
                                    Return MyResult
                                End If
                            End If

                        Case CAP.ICAP_XRESOLUTION, CAP.ICAP_YRESOLUTION
                            If tw_cap.ConType <> TWON.TWON_ONEVALUE Then
                                SetCondition(TWCC.TWCC_BADVALUE)
                                SetResult(TWRC.TWRC_FAILURE)
                                Return MyResult
                            Else
                                Dim pContainer As IntPtr = WinAPI.GlobalLock(tw_cap.hContainer)
                                Dim oneval As TW_ONEVALUE_FIX32 = Marshal.PtrToStructure(pContainer, GetType(TW_ONEVALUE_FIX32))
                                If pContainer Then WinAPI.GlobalUnlock(tw_cap.hContainer)
                                Logger.Write(DebugLogger.Level.Debug, False, "ItemType=" & CType(oneval.ItemType, TWTY).ToString)
                                If oneval.ItemType <> TWTY.TWTY_FIX32 Then
                                    SetCondition(TWCC.TWCC_BADVALUE)
                                    SetResult(TWRC.TWRC_FAILURE)
                                    Return MyResult
                                Else
                                    SetCap(ReqCap.Capability, oneval.Item, RequestSource.TWAIN)
                                    SetResult(TWRC.TWRC_SUCCESS)
                                    Return MyResult
                                End If
                            End If

                        Case Else
                            'XXX lots more CAPs to add here!

                            SetCondition(TWCC.TWCC_CAPUNSUPPORTED)
                            SetResult(TWRC.TWRC_FAILURE)
                            Return MyResult
                    End Select
                Case MSG.MSG_QUERYSUPPORT
                    If MyState < TwainState.DS_Opened Then
                        SetCondition(TWCC.TWCC_SEQERROR)
                        SetResult(TWRC.TWRC_FAILURE)
                        Return MyResult
                    End If
                    Dim tw_cap As TW_CAPABILITY = Marshal.PtrToStructure(_pData, GetType(TW_CAPABILITY))
                    Logger.Write(DebugLogger.Level.Debug, False, "Capability=" & CType(tw_cap.Cap, CAP).ToString & ", ContainerType=" & CType(tw_cap.ConType, TWON).ToString)

                    Dim ResultBits As Int32 = 0
                    Try
                        Dim ReqCap As TwainCapability = Caps(tw_cap.Cap)
                        ResultBits = ReqCap.SupportedOperations
                    Catch ex As Exception
                        ResultBits = 0 'return success with no supported operations
                    End Try

                    Dim oneval As TW_ONEVALUE
                    oneval.ItemType = TWTY.TWTY_INT32
                    oneval.Item = ResultBits
                    Dim pContainer As IntPtr = WinAPI.GlobalAlloc(WinAPI.GlobalAllocFlags.GHND, Marshal.SizeOf(oneval))
                    tw_cap.ConType = TWON.TWON_ONEVALUE
                    tw_cap.hContainer = pContainer
                    Marshal.StructureToPtr(oneval, pContainer, True)
                    Marshal.StructureToPtr(tw_cap, _pData, True)
                    SetResult(TWRC.TWRC_SUCCESS)
                    Return MyResult

                Case Else
                    'XXX lots more MSGs to add here!

                    SetCondition(TWCC.TWCC_BADPROTOCOL)
                    SetResult(TWRC.TWRC_FAILURE)
                    Return MyResult
            End Select
        End Function

        Private Function DG_CONTROL__DAT_USERINTERFACE(ByVal _MSG As MSG, ByVal _pData As IntPtr) As TWRC
            Select Case _MSG
                Case MSG.MSG_ENABLEDS
                    If MyState <> TwainState.DS_Opened Then
                        SetCondition(TWCC.TWCC_OPERATIONERROR)
                        SetResult(TWRC.TWRC_FAILURE)
                        Return MyResult
                    Else
                        Dim UserInterface As TW_USERINTERFACE = Marshal.PtrToStructure(_pData, GetType(TW_USERINTERFACE))
                        Logger.Write(DebugLogger.Level.Debug, False, "ShowUI = '" & UserInterface.ShowUI.ToString & "'")
                        MyForm.Parent = System.Windows.Forms.Form.FromHandle(UserInterface.hParent)
                        MyForm.Mode = FormMain.UIMode.Scan
                        Me.SetState(TwainState.DS_Enabled)
                        If CBool(UserInterface.ShowUI) Then
                            MyForm.Show()
                            MyForm.ButtonOK.Enabled = True
                        Else
                            'MyForm.ButtonOK.PerformClick() 'this doesn't work for some reason
                            Me.GUI_ButtonOK_Click(MyForm.ButtonOK, New System.Windows.Forms.MouseEventArgs(Windows.Forms.MouseButtons.Left, 1, 1, 1, 0))
                        End If

                        UserInterface.ModalUI = TW_FALSE
                        Marshal.StructureToPtr(UserInterface, _pData, True)

                        SetResult(TWRC.TWRC_SUCCESS)
                        Return MyResult
                    End If
                Case MSG.MSG_ENABLEDSUIONLY
                    If MyState <> TwainState.DS_Opened Then
                        SetCondition(TWCC.TWCC_OPERATIONERROR)
                        SetResult(TWRC.TWRC_FAILURE)
                        Return MyResult
                    Else
                        Dim UserInterface As TW_USERINTERFACE = Marshal.PtrToStructure(_pData, GetType(TW_USERINTERFACE))
                        MyForm.Parent = System.Windows.Forms.Form.FromHandle(UserInterface.hParent)
                        MyForm.Mode = FormMain.UIMode.Configure
                        MyForm.Show()
                        MyForm.ButtonOK.Enabled = True

                        UserInterface.ModalUI = TW_FALSE
                        Marshal.StructureToPtr(UserInterface, _pData, True)

                        Me.SetState(TwainState.DS_Enabled)
                        SetResult(TWRC.TWRC_SUCCESS)
                        Return MyResult
                    End If

                Case MSG.MSG_DISABLEDS
                    If MyState <> TwainState.DS_Enabled Then
                        SetCondition(TWCC.TWCC_SEQERROR)
                        SetResult(TWRC.TWRC_FAILURE)
                        Return MyResult
                    Else
                        If MyForm IsNot Nothing Then
                            MyForm.Hide()
                            MyForm.Parent = Nothing
                            'MyForm.Close()
                            'MyForm = Nothing
                        End If
                        Me.SetState(TwainState.DS_Opened)
                        SetResult(TWRC.TWRC_SUCCESS)
                        Return MyResult
                    End If
                Case Else
                    SetCondition(TWCC.TWCC_BADPROTOCOL)
                    SetResult(TWRC.TWRC_FAILURE)
                    Return MyResult
            End Select
        End Function

        Private Function DG_CONTROL__DAT_IDENTITY(ByVal _pOrigin As IntPtr, ByVal _MSG As MSG, ByVal _pData As IntPtr) As TWRC
            Select Case _MSG
                Case MSG.MSG_GET
                    Marshal.StructureToPtr(MyIdentity, _pData, True)
                    SetResult(TWRC.TWRC_SUCCESS)
                    Return MyResult
                Case MSG.MSG_OPENDS
                    If MyState <> TwainState.DSM_Opened Then
                        SetCondition(TWCC.TWCC_SEQERROR)
                        SetResult(TWRC.TWRC_FAILURE)
                        Return MyResult
                    Else
                        'keep a copy of the TW_IDENTITY of the application talking to us
                        MyIdentity = Marshal.PtrToStructure(_pData, GetType(TW_IDENTITY))
                        Dim NewAppIdentity As TW_IDENTITY
                        NewAppIdentity = Marshal.PtrToStructure(_pOrigin, GetType(TW_IDENTITY))
                        Logger.Write(DebugLogger.Level.Info, False, "MyIdentity.Id=" & MyIdentity.Id.ToString & ", MyIdentity.ProductName=" & MyIdentity.ProductName & ", MyIdentity.Manufacturer=" & MyIdentity.Manufacturer)
                        Logger.Write(DebugLogger.Level.Info, False, "AppIdentity.Id=" & NewAppIdentity.Id.ToString & ", AppIdentity.ProductName=" & NewAppIdentity.ProductName & ", AppIdentity.Manufacturer=" & NewAppIdentity.Manufacturer)

                        If AppIdentity.Id <> 0 Then
                            If NewAppIdentity.Id <> AppIdentity.Id Then
                                SetCondition(TWCC.TWCC_MAXCONNECTIONS)
                                SetResult(TWRC.TWRC_FAILURE)
                                Return MyResult
                            End If
                        End If

                        AppIdentity = NewAppIdentity

                        CurrentJob = New TWAINJob

                        If MyForm Is Nothing Then
                            MyForm = New FormMain
                            MyForm.TWAIN_Is_Active = True
                            MyForm.TWAINInstance = Me
                            AddHandler MyForm.ButtonOK.Click, AddressOf Me.GUI_ButtonOK_Click
                            AddHandler MyForm.ButtonCancel.Click, AddressOf Me.GUI_ButtonCancel_Click
                            AddHandler MyForm.FormClosing, AddressOf Me.GUI_FormClosing
                            'MyForm.CheckBoxBatchMode.Checked = CBool(Me.Caps(CAP.CAP_FEEDERENABLED).CurrentValue)
                            MyForm.CheckBoxBatchMode.Checked = CurrentSettings.ScanContinuously
                            AddHandler MyForm.CheckBoxBatchMode.CheckedChanged, AddressOf Me.GUI_CheckBoxBatchMode_CheckedChanged
                        End If

                        'Start talking SANE
                        If SANE Is Nothing Then SANE = New SANE_API

                        'If SANE isn't configured, launch the wizard.
                        If Not (CurrentSettings.HostIsValid(CurrentSettings.SANE.CurrentHost) AndAlso (CurrentSettings.SANE.CurrentDevice IsNot Nothing) AndAlso CurrentSettings.SANE.CurrentDevice.Length) Then
                            Dim f As New FormSANEHostWizard
                            If f.ShowDialog <> Windows.Forms.DialogResult.OK Then
                                Logger.Write(DebugLogger.Level.Debug, False, "User cancelled SANE host wizard")
                                SetCondition(TWCC.TWCC_BUMMER)
                                Return TWRC.TWRC_FAILURE
                            End If
                        End If

                        'SANE is configured now, so try to connect.  Launch the wizard on failure.
                        Do
                            Try
                                If net Is Nothing Then net = New System.Net.Sockets.TcpClient
                                If net IsNot Nothing Then
                                    net.ReceiveTimeout = CurrentSettings.SANE.CurrentHost.TCP_Timeout_ms
                                    net.SendTimeout = CurrentSettings.SANE.CurrentHost.TCP_Timeout_ms

                                    'net.SendBufferSize = 65536
                                    'net.ReceiveBufferSize = 65536

                                    Logger.Write(DebugLogger.Level.Debug, False, "TCPClient Send buffer length is " & net.SendBufferSize)
                                    Logger.Write(DebugLogger.Level.Debug, False, "TCPClient Receive buffer length is " & net.ReceiveBufferSize)

                                    Dim status As SANE_API.SANE_Status
                                    SANE.CurrentDevice = New SANE_API.CurrentDeviceInfo
                                    net.Connect(CurrentSettings.SANE.CurrentHost.NameOrAddress, CurrentSettings.SANE.CurrentHost.Port)
                                    status = SANE.Net_Init(net, CurrentSettings.SANE.CurrentHost.Username)
                                    Logger.Write(DebugLogger.Level.Debug, False, "Net_Init returned status '" & status.ToString & "'")
                                    If status = SANE_API.SANE_Status.SANE_STATUS_GOOD Then
                                        CurrentSettings.SANE.CurrentHost.Open = True
                                        Dim DeviceHandle As Integer
                                        status = SANE.Net_Open(net, CurrentSettings.SANE.CurrentDevice, DeviceHandle)
                                        Logger.Write(DebugLogger.Level.Debug, False, "Net_Open returned status '" & status.ToString & "'")

                                        If status = SANE_API.SANE_Status.SANE_STATUS_INVAL Then  'Auto-Locate
                                            If CurrentSettings.SANE.AutoLocateDevice IsNot Nothing AndAlso CurrentSettings.SANE.AutoLocateDevice.Length > 0 Then
                                                Logger.Write(DebugLogger.Level.Debug, False, "Attempting to auto-locate devices matching '" & CurrentSettings.SANE.AutoLocateDevice & "'")
                                                Dim Devices(-1) As SANE_API.SANE_Device
                                                status = SANE.Net_Get_Devices(net, Devices)
                                                If status = SANE_API.SANE_Status.SANE_STATUS_GOOD Then
                                                    For i As Integer = 0 To Devices.Length - 1
                                                        status = SANE_API.SANE_Status.SANE_STATUS_INVAL
                                                        If Devices(i).name.Trim.Length >= CurrentSettings.SANE.AutoLocateDevice.Length Then
                                                            If Devices(i).name.Trim.Substring(0, CurrentSettings.SANE.AutoLocateDevice.Length) = CurrentSettings.SANE.AutoLocateDevice Then
                                                                Logger.Write(DebugLogger.Level.Debug, False, "Auto-located device '" & Devices(i).name & "'; attempting to open...")
                                                                status = SANE.Net_Open(net, Devices(i).name, DeviceHandle)
                                                                Logger.Write(DebugLogger.Level.Debug, False, "Net_Open returned status '" & status.ToString & "'")
                                                                If status = SANE_API.SANE_Status.SANE_STATUS_GOOD Then CurrentSettings.SANE.CurrentDevice = Devices(i).name
                                                                Exit For
                                                            End If
                                                        End If
                                                    Next
                                                End If
                                            End If
                                        End If

                                        If status = SANE_API.SANE_Status.SANE_STATUS_GOOD Then
                                            SANE.CurrentDevice.Name = CurrentSettings.SANE.CurrentDevice
                                            SANE.CurrentDevice.Handle = DeviceHandle
                                            SANE.CurrentDevice.Open = True

                                            MyForm.GetOpts(True) 'must occur prior to reading GetDeviceConfigFileName()!

                                            CurrentSettings.SANE.CurrentDeviceINI = New IniFile
                                            Dim s As String = CurrentSettings.GetDeviceConfigFileName()
                                            If s IsNot Nothing AndAlso s.Length > 0 Then CurrentSettings.SANE.CurrentDeviceINI.Load(s)

                                            Import_SANE_Options()
                                            MyForm.SetUserDefaults()
                                            'MyForm.ButtonOK.Enabled = True
                                        End If
                                    End If

                                    If Not SANE.CurrentDevice.Open Then

                                        If CurrentSettings.SANE.CurrentHost.Open Then
                                            SANE.Net_Exit(net)
                                        End If

                                        If net.Connected Then
                                            Dim stream As System.Net.Sockets.NetworkStream = net.GetStream
                                            stream.Close()
                                            stream = Nothing
                                        End If

                                        If net.Connected Then net.Close()
                                        net = Nothing

                                        Dim f As New FormSANEHostWizard
                                        If f.ShowDialog <> Windows.Forms.DialogResult.OK Then
                                            'Logger.Write(DebugLogger.Level.Debug, False, "User cancelled SANE host wizard")
                                            'SetCondition(TWCC.TWCC_BUMMER)
                                            'Return TWRC.TWRC_FAILURE
                                            Throw New ApplicationException("Scanner was not configured")
                                        End If
                                    End If
                                End If
                            Catch ex As Exception
                                Logger.Write(DebugLogger.Level.Error_, True, ex.Message)
                                Try
                                    If SANE.CurrentDevice.Open Then
                                        SANE.Net_Close(net, SANE.CurrentDevice.Handle)
                                        SANE.CurrentDevice.Open = False
                                    End If
                                    If CurrentSettings.SANE.CurrentHost.Open Then
                                        SANE.Net_Exit(net)
                                    End If
                                    SetCondition(TWCC.TWCC_BUMMER)
                                    Return TWRC.TWRC_FAILURE
                                Catch exx As Exception
                                    SetCondition(TWCC.TWCC_BUMMER)
                                    Return TWRC.TWRC_FAILURE
                                End Try
                            Finally
                                Try
                                    If Not CurrentSettings.SANE.CurrentHost.Open Then
                                        If net IsNot Nothing Then
                                            If net.Connected Then
                                                Dim stream As System.Net.Sockets.NetworkStream = net.GetStream
                                                stream.Close()
                                                stream = Nothing
                                            End If
                                            If net.Connected Then net.Close()
                                            net = Nothing
                                        End If
                                    End If
                                Catch ex As Exception
                                    Logger.Write(DebugLogger.Level.Error_, True, ex.Message)
                                End Try
                            End Try
                        Loop While Not SANE.CurrentDevice.Open
                        Me.SetState(TwainState.DS_Opened)
                        SetResult(TWRC.TWRC_SUCCESS)
                        Return MyResult
                    End If
                Case MSG.MSG_CLOSEDS
                    If MyState = TwainState.DS_Opened Then
                        'XXX check for multiple connected applications

                        Try

                            If MyForm IsNot Nothing Then
                                MyForm.Got_MSG_CLOSEDS = True
                                MyForm.Close()
                                MyForm = Nothing
                            End If

                            If net IsNot Nothing Then
                                'If net.Connected Then
                                '    'If SANE.CurrentDevice.Open Then
                                '    '    SANE.Net_Close(net, SANE.CurrentDevice.Handle)
                                '    'End If
                                '    'If CurrentSettings.SANE.CurrentHost.Open Then
                                '    '    SANE.Net_Exit(net)
                                '    'End If

                                '    'Dim stream As System.Net.Sockets.NetworkStream = net.GetStream
                                '    'stream.Close()
                                '    'stream = Nothing
                                'End If
                                If net.Connected Then net.Close()
                            End If
                        Catch ex As Exception
                            Logger.Write(DebugLogger.Level.Error_, True, ex.Message)
                        Finally
                            CurrentSettings.SANE.CurrentHost.Open = False
                            net = Nothing
                        End Try

                        AppIdentity = Nothing
                        SetState(TwainState.DSM_Opened)
                    End If
                    SetResult(TWRC.TWRC_SUCCESS) 'return success even if it wasn't open
                    Return MyResult
                Case Else
                    Logger.Write(DebugLogger.Level.Warn, False, "***Unimplemented message type: " & _MSG.ToString & " ***")
                    SetCondition(TWCC.TWCC_BADPROTOCOL)
                    SetResult(TWRC.TWRC_FAILURE)
                    Return MyResult
            End Select
        End Function

        Private Function DG_CONTROL__DAT_EVENT(ByVal _MSG As MSG, ByVal _pData As IntPtr) As TWRC
            Select Case _MSG
                Case MSG.MSG_PROCESSEVENT
                    'If MyState < TwainState.DS_Enabled Then 'this is correct per TWAIN documentation.
                    If MyState < TwainState.DS_Opened Then 'XXX this is wrong per TWAIN documentation, but their Twacker tool does this.
                        Logger.Write(DebugLogger.Level.Warn, False, "MSG_PROCESSEVENT was received in illegal state: '" & MyState.ToString & "'")
                        SetCondition(TWCC.TWCC_SEQERROR)
                        SetResult(TWRC.TWRC_FAILURE)
                        Return MyResult
                    Else
                        If _pData = IntPtr.Zero Then
                            SetCondition(TWCC.TWCC_BADVALUE)
                            SetResult(TWRC.TWRC_FAILURE)
                            Return MyResult
                        Else
                            Dim e As TW_EVENT = Marshal.PtrToStructure(_pData, GetType(TW_EVENT))

                            'XXX Here we could set TWMessage to MSG_CLOSEDSREQ, MSG_CLOSEDSOK, MSG_XFERREADY, or MSG_DEVICEEVENT instead, but that's the DSM's job?
                            e.TWMessage = MSG.MSG_NULL

                            If e.pEvent = IntPtr.Zero Then
                                SetCondition(TWCC.TWCC_BADVALUE)
                                SetResult(TWRC.TWRC_FAILURE)
                                Return MyResult
                            Else
                                Dim hWnd As IntPtr = Marshal.ReadIntPtr(e.pEvent) 'read the first word from the specified address
                                Marshal.StructureToPtr(e, _pData, True)
                                If hWnd = MyForm.Handle Then
                                    WinAPI.IsDialogMessage(hWnd, e.pEvent)
                                    SetResult(TWRC.TWRC_DSEVENT)
                                    Return MyResult
                                Else
                                    SetResult(TWRC.TWRC_NOTDSEVENT)
                                    Return MyResult
                                End If
                            End If
                        End If
                    End If
                Case Else
                    Logger.Write(DebugLogger.Level.Warn, False, "***Unimplemented message type: " & _MSG.ToString & " ***")
                    SetCondition(TWCC.TWCC_BADPROTOCOL)
                    SetResult(TWRC.TWRC_FAILURE)
                    Return MyResult
            End Select
        End Function

        Private Function SafeString(ByVal o As Object) As String
            If o Is Nothing Then Return "Nothing"
            Return o.ToString
        End Function

        Public Sub New()
            Dim UseRoamingAppData As Boolean = False
            Try
                UseRoamingAppData = My.Settings.UseRoamingAppData
            Catch ex As Exception
            End Try

            If Logger Is Nothing Then Logger = New DebugLogger(UseRoamingAppData)

            'Dim MyName As System.Reflection.AssemblyName = System.Reflection.Assembly.GetExecutingAssembly.GetName
            'Dim strPath As String = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase)
            Logger.Write(DebugLogger.Level.Info, False, "codebase=" & System.Reflection.Assembly.GetExecutingAssembly().CodeBase)

            If CurrentSettings Is Nothing Then CurrentSettings = New SharedSettings(UseRoamingAppData)

            Logger.Write(DebugLogger.Level.Info, False, CurrentSettings.ProductName.FullName & " object created by '" & System.Environment.GetCommandLineArgs()(0) & "'")

            Me.SetState(TwainState.DSM_Opened)
            Me.SetCondition(TWCC.TWCC_SUCCESS)

            MyIdentity = New TW_IDENTITY

            MyIdentity.Id = 0
            MyIdentity.Version.MajorNum = CurrentSettings.ProductName.Version.Major
            MyIdentity.Version.MinorNum = CurrentSettings.ProductName.Version.Minor
            MyIdentity.Version.Language = TWLG.TWLG_ENGLISH
            MyIdentity.Version.Country = TWCY.TWCY_USA
            MyIdentity.Version.Info = MyIdentity.Version.MajorNum & "." & MyIdentity.Version.MinorNum & "." & CurrentSettings.ProductName.Version.Build
            'Starting with TWAIN 1.9 all sources must support non-gui control.
            MyIdentity.ProtocolMajor = 1
            MyIdentity.ProtocolMinor = 9
            MyIdentity.SupportedGroups = DG.DG_IMAGE Or DG.DG_CONTROL
            If MyIdentity.ProtocolMajor > 1 Then MyIdentity.SupportedGroups = MyIdentity.SupportedGroups Or DF.DF_DS2
            MyIdentity.Manufacturer = CType(System.Reflection.Assembly.GetExecutingAssembly.GetCustomAttributes(GetType(System.Reflection.AssemblyCompanyAttribute), False)(0), System.Reflection.AssemblyCompanyAttribute).Company
            MyIdentity.ProductFamily = CurrentSettings.ProductName.Name
            MyIdentity.ProductName = CurrentSettings.ProductName.Name

            MyTWAINversion = MyIdentity.ProtocolMajor + (MyIdentity.ProtocolMinor / 10)
            Logger.Write(DebugLogger.Level.Info, False, "Reporting my TWAIN version as '" & MyTWAINversion.ToString & "'")

            Me.InitCaps()
        End Sub

        Protected Overrides Sub Finalize()
            ' Destructor
            Try
                'Marshal.FreeHGlobal(pMyIdentity)
                Logger.Write(DebugLogger.Level.Debug, False, "Object destroyed")
            Catch ex As Exception

            Finally
                MyBase.Finalize()
            End Try
        End Sub

        Private Sub GUI_ButtonOK_Click(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs)
            Logger.Write(DebugLogger.Level.Debug, False, "")
            If MyForm.Mode = FormMain.UIMode.Scan Then
                SetState(TwainState.DS_Xfer_Ready)
                Send_TWAIN_Message(MyIdentity, AppIdentity, DG.DG_CONTROL, DAT.DAT_NULL, MSG.MSG_XFERREADY, Nothing)
            Else
                Send_TWAIN_Message(MyIdentity, AppIdentity, DG.DG_CONTROL, DAT.DAT_NULL, MSG.MSG_CLOSEDSOK, Nothing)
            End If
        End Sub

        Private Sub GUI_ButtonCancel_Click(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs)
            Logger.Write(DebugLogger.Level.Debug, False, "")
            Send_TWAIN_Message(MyIdentity, AppIdentity, DG.DG_CONTROL, DAT.DAT_NULL, MSG.MSG_CLOSEDSREQ, Nothing)
        End Sub

        Private Sub GUI_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs)
            If Not sender.Got_MSG_CLOSEDS Then
                Logger.Write(DebugLogger.Level.Debug, False, "Intercepted FormClosing event; sending MSG_CLOSEDSREQ instead")
                e.Cancel = True
                Send_TWAIN_Message(MyIdentity, AppIdentity, DG.DG_CONTROL, DAT.DAT_NULL, MSG.MSG_CLOSEDSREQ, Nothing)
            End If
        End Sub

        Private Sub GUI_CheckBoxBatchMode_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs)
            If sender.Checked Then
                Me.SetCap(CAP.CAP_FEEDERENABLED, TW_TRUE, RequestSource.TWAIN)
            Else
                Me.SetCap(CAP.CAP_FEEDERENABLED, TW_FALSE, RequestSource.TWAIN)
            End If
        End Sub

        'Private Function CreateObj(Of T)(ByVal ObjType As T)
        '    Dim obj As T
        '    Return obj
        'End Function

        Public Sub SetCap(ByVal Capability As CAP, ByVal NewValue As Object, ByVal Source As RequestSource)
            Try
                Dim ReqCap As TwainCapability = Caps(Capability)
                Dim OldVal As Object = ReqCap.CurrentValue
                Dim EnumTypeStr As String
                If ReqCap.EnumType IsNot Nothing Then
                    EnumTypeStr = ReqCap.EnumType.ToString
                Else
                    EnumTypeStr = "Nothing"
                End If
                Logger.Write(DebugLogger.Level.Debug, False, "Capability: '" & Capability.ToString & "', Target Type = '" & ReqCap.DataType.ToString & "', Target Enum = '" & EnumTypeStr & "', NewValue Type = '" & NewValue.GetType.ToString & "', NewValue = '" & NewValue.ToString & "'")

                Dim OldValStr As String = Nothing
                Dim NewValStr As String = Nothing

                'If NewValue.GetType <> OldVal.GetType Then
                'Logger.Write(DebugLogger.Level.Warn, False, "Value type mismatch; attempting to convert from '" & NewValue.GetType.ToString & "' to '" & OldVal.GetType.ToString & "'")
                'Logger.Write(DebugLogger.Level.Warn, False, "Value type mismatch; attempting to convert from '" & NewValue.GetType.ToString & "' to '" & ReqCap.DataType.ToString & "'")

                'Select Case OldVal.GetType
                '    Case GetType(Int32)
                Select Case ReqCap.DataType
                    Case TWTY.TWTY_BOOL
                        Select Case NewValue.ToString.ToUpper.Trim
                            Case "TRUE"
                                NewValue = TW_TRUE
                            Case "FALSE"
                                NewValue = TW_FALSE
                            Case Else
                                Dim n As Int32
                                If Int32.TryParse(NewValue.ToString, n) Then
                                    If n = 0 Then NewValue = TW_FALSE Else NewValue = TW_TRUE
                                Else
                                    Throw New ApplicationException("Data type conversion failed")
                                End If
                        End Select
                        Select Case OldVal
                            Case TW_FALSE
                                OldValStr = "TW_FALSE"
                            Case TW_TRUE
                                OldValStr = "TW_TRUE"
                            Case Else
                                OldValStr = "Invalid: '" & OldVal.ToString & "'"
                        End Select
                        Select Case NewValue
                            Case TW_FALSE
                                NewValStr = "TW_FALSE"
                            Case TW_TRUE
                                NewValStr = "TW_TRUE"
                            Case Else
                                NewValStr = "Invalid: '" & NewValue.ToString & "'"
                        End Select
                    Case TWTY.TWTY_FIX32
                        Select Case NewValue.GetType
                            Case GetType(TW_FIX32)
                                'do nothing
                                'Case GetType(Double)
                                '    NewValue = FloatToFIX32(NewValue)
                                'Case GetType(Decimal), GetType(Single), GetType(Int64), GetType(UInt64), GetType(Int32), GetType(UInt32), GetType(Int16), GetType(UInt16)
                                '    NewValue = FloatToFIX32(CDbl(NewValue))
                            Case Else
                                Dim d As Double
                                'Logger.Write(DebugLogger.Level.Debug, False, "1")
                                If Double.TryParse(NewValue.ToString, d) Then
                                    'Logger.Write(DebugLogger.Level.Debug, False, "2")
                                    NewValue = FloatToFIX32(d)
                                    'Logger.Write(DebugLogger.Level.Debug, False, "3")
                                Else
                                    Throw New ApplicationException("Data type conversion failed")
                                End If
                        End Select
                        'Logger.Write(DebugLogger.Level.Debug, False, "4")
                        OldValStr = FIX32ToFloat(OldVal).ToString
                        'Logger.Write(DebugLogger.Level.Debug, False, "5")
                        NewValStr = FIX32ToFloat(NewValue).ToString
                        'Logger.Write(DebugLogger.Level.Debug, False, "6")
                    Case TWTY.TWTY_INT8, TWTY.TWTY_INT16, TWTY.TWTY_INT32, TWTY.TWTY_UINT8, TWTY.TWTY_UINT16, TWTY.TWTY_UINT32
                        Select Case NewValue.GetType
                            Case GetType(String)
                                If ReqCap.EnumType IsNot Nothing Then
                                    Select Case ReqCap.EnumType
                                        Case GetType(TWBO)
                                            Dim o As TWBO
                                            If [Enum].TryParse(NewValue.ToString, True, o) Then
                                                NewValue = o
                                            Else
                                                Throw New ApplicationException("Unable to interpret '" & NewValue.ToString & "' as type '" & ReqCap.EnumType.ToString & "'")
                                            End If
                                        Case GetType(TWCC)
                                            Dim o As TWCC
                                            If [Enum].TryParse(NewValue.ToString, True, o) Then
                                                NewValue = o
                                            Else
                                                Throw New ApplicationException("Unable to interpret '" & NewValue.ToString & "' as type '" & ReqCap.EnumType.ToString & "'")
                                            End If
                                        Case GetType(TWCP)
                                            Dim o As TWCP
                                            If [Enum].TryParse(NewValue.ToString, True, o) Then
                                                NewValue = o
                                            Else
                                                Throw New ApplicationException("Unable to interpret '" & NewValue.ToString & "' as type '" & ReqCap.EnumType.ToString & "'")
                                            End If
                                        Case GetType(TWCY)
                                            Dim o As TWCY
                                            If [Enum].TryParse(NewValue.ToString, True, o) Then
                                                NewValue = o
                                            Else
                                                Throw New ApplicationException("Unable to interpret '" & NewValue.ToString & "' as type '" & ReqCap.EnumType.ToString & "'")
                                            End If
                                        Case GetType(TWDX)
                                            Dim o As TWDX
                                            If [Enum].TryParse(NewValue.ToString, True, o) Then
                                                NewValue = o
                                            Else
                                                Throw New ApplicationException("Unable to interpret '" & NewValue.ToString & "' as type '" & ReqCap.EnumType.ToString & "'")
                                            End If
                                        Case GetType(TWLG)
                                            Dim o As TWLG
                                            If [Enum].TryParse(NewValue.ToString, True, o) Then
                                                NewValue = o
                                            Else
                                                Throw New ApplicationException("Unable to interpret '" & NewValue.ToString & "' as type '" & ReqCap.EnumType.ToString & "'")
                                            End If
                                        Case GetType(TWMF)
                                            Dim o As TWMF
                                            If [Enum].TryParse(NewValue.ToString, True, o) Then
                                                NewValue = o
                                            Else
                                                Throw New ApplicationException("Unable to interpret '" & NewValue.ToString & "' as type '" & ReqCap.EnumType.ToString & "'")
                                            End If
                                        Case GetType(TWON)
                                            Dim o As TWON
                                            If [Enum].TryParse(NewValue.ToString, True, o) Then
                                                NewValue = o
                                            Else
                                                Throw New ApplicationException("Unable to interpret '" & NewValue.ToString & "' as type '" & ReqCap.EnumType.ToString & "'")
                                            End If
                                        Case GetType(TWPC)
                                            Dim o As TWPC
                                            If [Enum].TryParse(NewValue.ToString, True, o) Then
                                                NewValue = o
                                            Else
                                                Throw New ApplicationException("Unable to interpret '" & NewValue.ToString & "' as type '" & ReqCap.EnumType.ToString & "'")
                                            End If
                                        Case GetType(TWPF)
                                            Dim o As TWPF
                                            If [Enum].TryParse(NewValue.ToString, True, o) Then
                                                NewValue = o
                                            Else
                                                Throw New ApplicationException("Unable to interpret '" & NewValue.ToString & "' as type '" & ReqCap.EnumType.ToString & "'")
                                            End If
                                        Case GetType(TWPT)
                                            Dim o As TWPT
                                            If [Enum].TryParse(NewValue.ToString, True, o) Then
                                                NewValue = o
                                            Else
                                                Throw New ApplicationException("Unable to interpret '" & NewValue.ToString & "' as type '" & ReqCap.EnumType.ToString & "'")
                                            End If
                                        Case GetType(TWQC)
                                            Dim o As TWQC
                                            If [Enum].TryParse(NewValue.ToString, True, o) Then
                                                NewValue = o
                                            Else
                                                Throw New ApplicationException("Unable to interpret '" & NewValue.ToString & "' as type '" & ReqCap.EnumType.ToString & "'")
                                            End If
                                        Case GetType(TWRC)
                                            Dim o As TWRC
                                            If [Enum].TryParse(NewValue.ToString, True, o) Then
                                                NewValue = o
                                            Else
                                                Throw New ApplicationException("Unable to interpret '" & NewValue.ToString & "' as type '" & ReqCap.EnumType.ToString & "'")
                                            End If
                                        Case GetType(TWSX)
                                            Dim o As TWSX
                                            If [Enum].TryParse(NewValue.ToString, True, o) Then
                                                NewValue = o
                                            Else
                                                Throw New ApplicationException("Unable to interpret '" & NewValue.ToString & "' as type '" & ReqCap.EnumType.ToString & "'")
                                            End If
                                        Case GetType(TWTY)
                                            Dim o As TWTY
                                            If [Enum].TryParse(NewValue.ToString, True, o) Then
                                                NewValue = o
                                            Else
                                                Throw New ApplicationException("Unable to interpret '" & NewValue.ToString & "' as type '" & ReqCap.EnumType.ToString & "'")
                                            End If
                                        Case GetType(TWUN)
                                    End Select
                                Else
                                    Dim int As Int64
                                    If Int64.TryParse(NewValue, int) Then
                                        NewValue = int
                                    Else
                                        Throw New ApplicationException("Data type conversion failed")
                                    End If
                                End If
                            Case Else
                                Dim int As Int64
                                If Int64.TryParse(NewValue, int) Then
                                    NewValue = int
                                Else
                                    Throw New ApplicationException("Data type conversion failed")
                                End If
                        End Select
                        If (ReqCap.EnumType IsNot Nothing) AndAlso (NewValue.GetType <> OldVal.GetType) Then
                            Logger.Write(DebugLogger.Level.Debug, False, "Attempting CTypeDynamic for type '" & ReqCap.EnumType.ToString & "'")
                            NewValue = CTypeDynamic(NewValue, ReqCap.EnumType)
                        End If
                        OldValStr = OldVal.ToString
                        NewValStr = NewValue.ToString

                        'End Select

                        'XXX lots more data type conversions to code here


                End Select


                'End If

                ReqCap.CurrentValue = NewValue
                Caps(ReqCap.Capability) = ReqCap
                Logger.Write(DebugLogger.Level.Debug, False, ReqCap.Capability.ToString & ": " & OldValStr & " --> " & NewValStr)

                'If ReqCap.Capability = CAP.CAP_FEEDERENABLED Then
                '    MyForm.CheckBoxBatchMode.Enabled = CBool(ReqCap.CurrentValue)
                'End If

                If Source <> RequestSource.SANE Then 'if it came from SANE we don't want to give it back or we'll have an endless loop
                    If Not SetSANECaps(Capability, NewValue) Then
                        Logger.Write(DebugLogger.Level.Warn, False, "Error setting SANE option for capability '" & ReqCap.Capability.ToString & "'")
                    End If
                End If

            Catch ex As Exception
                Logger.Write(DebugLogger.Level.Error_, False, "Error setting capability '" & Capability.ToString & "': " & ex.Message)
            End Try
        End Sub

        Public Function SetSANECaps(ByVal Capability As CAP, ByVal NewValue As Object) As Boolean
            'Return true if the cap was mapped, otherwise false
            SetSANECaps = False
            If NewValue.GetType Is GetType(TW_FIX32) Then NewValue = FIX32ToFloat(NewValue)
            If CurrentSettings.SANE.CurrentDeviceINI IsNot Nothing Then
                Dim s As String = CurrentSettings.SANE.CurrentDeviceINI.GetKeyValue("TWAIN." & Capability.ToString, "SANE." & NewValue.ToString.Replace(" ", ""))
                'If there wasn't a TWAIN mapping for the specific value that was set, look for a general mapping.
                If (s Is Nothing) OrElse (s.Length = 0) Then s = CurrentSettings.SANE.CurrentDeviceINI.GetKeyValue("TWAIN." & Capability.ToString, "SANE")
                If s IsNot Nothing AndAlso s.Length Then
                    Dim caps() As String = s.Split(";")
                    Dim capName(caps.Length - 1) As String
                    Dim capVal(caps.Length - 1) As String
                    For i = 0 To caps.Length - 1
                        Dim ss() As String = caps(i).Split(",")
                        If ss.Length = 2 Then
                            capName(i) = ss(0).Trim.ToUpper
                            capVal(i) = ss(1).Trim.ToUpper
                            If SANE IsNot Nothing Then
                                If capVal(i) = "#" Then capVal(i) = NewValue.ToString
                                Logger.Write(DebugLogger.Level.Debug, False, "SANE Option = '" & capName(i) & "', Value = '" & capVal(i) & "'")
                                If MyForm.SetSANEOption(capName(i), {capVal(i)}) Then
                                    SetSANECaps = True
                                Else
                                    Logger.Write(DebugLogger.Level.Warn, False, "Error setting SANE option.")
                                    Return False
                                End If
                            End If
                        Else
                            Logger.Write(DebugLogger.Level.Warn, False, "Format of capability map is not correct")
                            Return False
                        End If
                    Next
                Else
                    Logger.Write(DebugLogger.Level.Debug, False, "No capability mapping was found for '" & Capability.ToString & "'")
                    Return False
                End If
            End If
        End Function

        Private Sub Import_SANE_Options()
            Logger.Write(DebugLogger.Level.Debug, False, "begin")
            Try
                'XXX set up general options like from Device_Appears_To_Have_ADF()

                If Not CurrentSettings.ScanContinuouslyUserConfigured Then
                    CurrentSettings.ScanContinuously = Device_Appears_To_Have_ADF() AndAlso Device_Appears_To_Have_ADF_Enabled()
                    'MyForm.CheckBoxBatchMode.Checked = CurrentSettings.ScanContinuously
                    SetCap(CAP.CAP_AUTOFEED, TW_TRUE, RequestSource.SANE) 'XXX applications should set this, but they don't seem to.
                End If

                If Device_Appears_To_Have_Duplex() Then 'XXX account for a setting in the ini file
                    SetCap(CAP.CAP_DUPLEX, TWDX.TWDX_1PASSDUPLEX, RequestSource.SANE)
                End If

                'Map SANE Well-Known Options
                Dim minx, maxx, miny, maxy, res_dpi As Double
                Dim xyunit As SANE_API.SANE_Unit = SANE_API.SANE_Unit.SANE_UNIT_NONE
                For i As Integer = 1 To SANE.CurrentDevice.OptionDescriptors.Count - 1 'skip the first option, which is just the option count
                    Dim TWAINcap As String = Nothing
                    Dim tlx, tly, brx, bry As Double
                    Select Case SANE.CurrentDevice.OptionDescriptors(i).name.ToLower
                        Case "resolution"
                            'TWAINcap = CAP.ICAP_XRESOLUTION.ToString & ", " & CAP.ICAP_YRESOLUTION.ToString & ", " & CAP.ICAP_XNATIVERESOLUTION.ToString & ", " & CAP.ICAP_YNATIVERESOLUTION.ToString
                            res_dpi = SANE.CurrentDevice.OptionValues(i)(0)
                            Dim caparray() As CAP = {CAP.ICAP_XRESOLUTION, CAP.ICAP_YRESOLUTION, CAP.ICAP_XNATIVERESOLUTION, CAP.ICAP_YNATIVERESOLUTION}
                            For Each icap As CAP In caparray
                                SetCap(icap, FloatToFIX32(res_dpi), RequestSource.SANE)
                            Next

                            'ICAP_UNITS must be set to TWUN_INCHES since SANE 'resolution' is always in DPI.
                            SetCap(CAP.ICAP_UNITS, TWUN.TWUN_INCHES, RequestSource.SANE)

                        Case "preview"  'No reason to map this
                        Case "tl-x"
                            tlx = SANE.CurrentDevice.OptionValues(i)(0)
                        Case "tl-y"
                            tly = SANE.CurrentDevice.OptionValues(i)(0)
                        Case "br-x"
                            brx = SANE.CurrentDevice.OptionValues(i)(0)
                            Select Case SANE.CurrentDevice.OptionDescriptors(i).constraint_type
                                Case SANE_API.SANE_Constraint_Type.SANE_CONSTRAINT_RANGE
                                    Dim n As Integer = SANE.CurrentDevice.OptionDescriptors(i).constraint.range.min
                                    minx = IIf(SANE.CurrentDevice.OptionDescriptors(i).type = SANE_API.SANE_Value_Type.SANE_TYPE_FIXED, SANE.SANE_UNFIX(n), n)
                                    n = SANE.CurrentDevice.OptionDescriptors(i).constraint.range.max
                                    maxx = IIf(SANE.CurrentDevice.OptionDescriptors(i).type = SANE_API.SANE_Value_Type.SANE_TYPE_FIXED, SANE.SANE_UNFIX(n), n)
                                Case SANE_API.SANE_Constraint_Type.SANE_CONSTRAINT_WORD_LIST
                                    Dim Words(SANE.CurrentDevice.OptionDescriptors(i).constraint.word_list.Length - 1) As Integer
                                    Array.Copy(SANE.CurrentDevice.OptionDescriptors(i).constraint.word_list, Words, Words.Length)
                                    Array.Sort(Words)
                                    minx = IIf(SANE.CurrentDevice.OptionDescriptors(i).type = SANE_API.SANE_Value_Type.SANE_TYPE_FIXED, SANE.SANE_UNFIX(Words(0)), Words(0))
                                    maxx = IIf(SANE.CurrentDevice.OptionDescriptors(i).type = SANE_API.SANE_Value_Type.SANE_TYPE_FIXED, SANE.SANE_UNFIX(Words(Words.Length - 1)), Words(Words.Length - 1))
                            End Select
                            xyunit = SANE.CurrentDevice.OptionDescriptors(i).unit
                        Case "br-y"
                            bry = SANE.CurrentDevice.OptionValues(i)(0)
                            Select Case SANE.CurrentDevice.OptionDescriptors(i).constraint_type
                                Case SANE_API.SANE_Constraint_Type.SANE_CONSTRAINT_RANGE
                                    Dim n As Integer = SANE.CurrentDevice.OptionDescriptors(i).constraint.range.min
                                    miny = IIf(SANE.CurrentDevice.OptionDescriptors(i).type = SANE_API.SANE_Value_Type.SANE_TYPE_FIXED, SANE.SANE_UNFIX(n), n)
                                    n = SANE.CurrentDevice.OptionDescriptors(i).constraint.range.max
                                    maxy = IIf(SANE.CurrentDevice.OptionDescriptors(i).type = SANE_API.SANE_Value_Type.SANE_TYPE_FIXED, SANE.SANE_UNFIX(n), n)
                                Case SANE_API.SANE_Constraint_Type.SANE_CONSTRAINT_WORD_LIST
                                    Dim Words(SANE.CurrentDevice.OptionDescriptors(i).constraint.word_list.Length - 1) As Integer
                                    Array.Copy(SANE.CurrentDevice.OptionDescriptors(i).constraint.word_list, Words, Words.Length)
                                    Array.Sort(Words)
                                    miny = IIf(SANE.CurrentDevice.OptionDescriptors(i).type = SANE_API.SANE_Value_Type.SANE_TYPE_FIXED, SANE.SANE_UNFIX(Words(0)), Words(0))
                                    miny = IIf(SANE.CurrentDevice.OptionDescriptors(i).type = SANE_API.SANE_Value_Type.SANE_TYPE_FIXED, SANE.SANE_UNFIX(Words(Words.Length - 1)), Words(Words.Length - 1))
                            End Select
                    End Select
                Next

                If (maxx > 0) And (maxy > 0) And ((xyunit = SANE_API.SANE_Unit.SANE_UNIT_PIXEL) Or (xyunit = SANE_API.SANE_Unit.SANE_UNIT_MM)) Then
                    Dim PhysicalWidth As Double = maxx - minx
                    Dim PhysicalLength As Double = maxy - miny
                    'internally we want to store everything as inches for TWAIN
                    Select Case xyunit
                        Case SANE_API.SANE_Unit.SANE_UNIT_MM
                            PhysicalWidth = MMToInches(PhysicalWidth)
                            PhysicalLength = MMToInches(PhysicalLength)
                        Case SANE_API.SANE_Unit.SANE_UNIT_PIXEL
                            'XXX no way to test this without a backend that reports dimensions in pixels
                            If res_dpi > 0 Then
                                PhysicalWidth = PhysicalWidth / res_dpi
                                PhysicalLength = PhysicalLength / res_dpi
                            Else
                                Logger.Write(DebugLogger.Level.Warn, False, "Unable to convert pixels to inches because dpi is unknown; using defaults instead")
                                PhysicalWidth = 8.5
                                PhysicalLength = 11
                            End If
                    End Select
                    Logger.Write(DebugLogger.Level.Debug, False, "physical size = " & PhysicalWidth.ToString & " x " & PhysicalLength.ToString & " inches")

                    SetCap(CAP.ICAP_UNITS, TWUN.TWUN_INCHES, RequestSource.SANE)
                    SetCap(CAP.ICAP_PHYSICALWIDTH, FloatToFIX32(PhysicalWidth), RequestSource.SANE)
                    SetCap(CAP.ICAP_PHYSICALHEIGHT, FloatToFIX32(PhysicalLength), RequestSource.SANE)

                End If

            Catch ex As Exception
                Logger.Write(DebugLogger.Level.Error_, True, ex.Message)
            End Try
            Logger.Write(DebugLogger.Level.Debug, False, "end")
        End Sub

        Private Function InchesToMM(ByVal Inches As Double) As Double
            Return Inches * 25.4
        End Function

        Private Function MMToInches(ByVal mm As Double) As Double
            Return mm / 25.4
        End Function

        Private Function InchesToCM(ByVal Inches As Double) As Double
            Return Inches * 2.54
        End Function

        Private Function CMToInches(ByVal cm As Double) As Double
            Return cm / 2.54
        End Function

        Public Function FloatToFIX32(ByVal floater As Double) As TW_FIX32
            Dim Fix32_value As TW_FIX32
            Dim sign As Boolean = (floater < 0)
            Dim value As Int32 = CType(floater * 65536.0 + IIf(sign, -0.5, 0.5), Int32)
            Fix32_value.Whole = CType(value >> 16, Int16)
            Fix32_value.Frac = CType(value And &HFFFFUS, UInt16)
            Return Fix32_value
        End Function

        Public Function FIX32ToFloat(ByVal _fix32 As TW_FIX32) As Double
            Return CType(_fix32.Whole, Double) + CType(_fix32.Frac / 65536.0, Double)
        End Function

        Private Sub InitCaps()
            Logger.Write(DebugLogger.Level.Debug, False, "")

            Dim tc As New TwainCapability
            tc.Capability = CAP.CAP_AUTOFEED
            tc.DataType = TWTY.TWTY_BOOL
            tc.EnumType = Nothing
            tc.DefaultValue = TW_FALSE
            tc.CurrentValue = tc.DefaultValue
            tc.SupportedOperations = TWQC.TWQC_ALL
            Caps.Add(tc.Capability, tc)

            tc = New TwainCapability
            tc.Capability = CAP.CAP_ENABLEDSUIONLY
            tc.DataType = TWTY.TWTY_BOOL
            tc.DefaultValue = TW_TRUE
            tc.CurrentValue = tc.DefaultValue
            tc.SupportedOperations = TWQC.TWQC_GET
            Caps.Add(tc.Capability, tc)

            tc = New TwainCapability
            tc.Capability = CAP.CAP_DUPLEX
            tc.DataType = TWTY.TWTY_UINT16
            tc.EnumType = GetType(TWDX)
            tc.DefaultValue = TWDX.TWDX_NONE
            tc.CurrentValue = tc.DefaultValue
            tc.SupportedOperations = TWQC.TWQC_GET
            Caps.Add(tc.Capability, tc)

            tc = New TwainCapability
            tc.Capability = CAP.CAP_DUPLEXENABLED
            tc.DataType = TWTY.TWTY_BOOL
            tc.DefaultValue = TW_FALSE
            tc.CurrentValue = tc.DefaultValue
            tc.SupportedOperations = TWQC.TWQC_ALL
            Caps.Add(tc.Capability, tc)

            tc = New TwainCapability
            tc.Capability = CAP.CAP_FEEDERENABLED
            tc.DataType = TWTY.TWTY_BOOL
            tc.DefaultValue = TW_FALSE
            tc.CurrentValue = tc.DefaultValue
            tc.SupportedOperations = TWQC.TWQC_ALL
            Caps.Add(tc.Capability, tc)

            tc = New TwainCapability
            tc.Capability = CAP.CAP_PAPERDETECTABLE
            tc.DataType = TWTY.TWTY_BOOL
            tc.DefaultValue = TW_FALSE
            tc.CurrentValue = tc.DefaultValue
            tc.SupportedOperations = TWQC.TWQC_ALLGET
            Caps.Add(tc.Capability, tc)

            tc = New TwainCapability
            tc.Capability = CAP.CAP_FEEDERLOADED
            tc.DataType = TWTY.TWTY_BOOL
            tc.DefaultValue = TW_TRUE
            tc.CurrentValue = tc.DefaultValue
            tc.SupportedOperations = TWQC.TWQC_ALLGET
            Caps.Add(tc.Capability, tc)

            tc = New TwainCapability
            tc.Capability = CAP.CAP_SUPPORTEDCAPS
            tc.DataType = TWTY.TWTY_UINT16
            tc.DefaultValue = 0
            tc.CurrentValue = tc.DefaultValue
            tc.SupportedOperations = TWQC.TWQC_GET
            Caps.Add(tc.Capability, tc)

            tc = New TwainCapability
            tc.Capability = CAP.CAP_UICONTROLLABLE
            tc.DataType = TWTY.TWTY_BOOL
            tc.DefaultValue = IIf(MyTWAINversion > 1.8, TW_TRUE, TW_FALSE)
            tc.CurrentValue = tc.DefaultValue
            tc.SupportedOperations = TWQC.TWQC_GET
            Caps.Add(tc.Capability, tc)

            tc = New TwainCapability
            tc.Capability = CAP.CAP_CUSTOMDSDATA
            tc.DataType = TWTY.TWTY_BOOL
            tc.DefaultValue = IIf(MyTWAINversion > 1.8, TW_TRUE, TW_FALSE)
            tc.CurrentValue = tc.DefaultValue
            tc.SupportedOperations = TWQC.TWQC_GET Or TWQC.TWQC_SET
            Caps.Add(tc.Capability, tc)

            tc = New TwainCapability
            tc.Capability = CAP.CAP_XFERCOUNT
            tc.DataType = TWTY.TWTY_INT16
            tc.DefaultValue = -1
            tc.CurrentValue = tc.DefaultValue
            tc.SupportedOperations = TWQC.TWQC_ALL
            Caps.Add(tc.Capability, tc)

            tc = New TwainCapability
            tc.Capability = CAP.ICAP_BITDEPTH
            tc.DefaultValue = TWTY.TWTY_UINT16
            tc.DefaultValue = 8
            tc.CurrentValue = tc.DefaultValue
            tc.SupportedOperations = TWQC.TWQC_ALL
            Caps.Add(tc.Capability, tc)

            tc = New TwainCapability
            tc.Capability = CAP.ICAP_BITORDER
            tc.DataType = TWTY.TWTY_UINT16
            tc.EnumType = GetType(TWBO)
            tc.DefaultValue = TWBO.TWBO_MSBFIRST
            tc.CurrentValue = tc.DefaultValue
            tc.SupportedOperations = TWQC.TWQC_ALL
            Caps.Add(tc.Capability, tc)

            tc = New TwainCapability
            tc.Capability = CAP.ICAP_COMPRESSION
            tc.DataType = TWTY.TWTY_UINT16
            tc.EnumType = GetType(TWCP)
            tc.DefaultValue = TWCP.TWCP_NONE
            tc.CurrentValue = tc.DefaultValue
            'XXX Need an array of supported values here
            tc.SupportedOperations = TWQC.TWQC_ALLGET
            Caps.Add(tc.Capability, tc)

            tc = New TwainCapability
            tc.Capability = CAP.ICAP_PLANARCHUNKY
            tc.DataType = TWTY.TWTY_UINT16
            tc.EnumType = GetType(TWPC)
            tc.DefaultValue = TWPC.TWPC_CHUNKY
            tc.CurrentValue = tc.DefaultValue
            tc.SupportedOperations = TWQC.TWQC_ALLGET
            Caps.Add(tc.Capability, tc)

            tc = New TwainCapability
            tc.Capability = CAP.ICAP_PHYSICALHEIGHT
            tc.DataType = TWTY.TWTY_FIX32
            tc.DefaultValue = FloatToFIX32(11.0R)
            tc.CurrentValue = tc.DefaultValue
            tc.SupportedOperations = TWQC.TWQC_ALLGET
            Caps.Add(tc.Capability, tc)

            tc = New TwainCapability
            tc.Capability = CAP.ICAP_PHYSICALWIDTH
            tc.DataType = TWTY.TWTY_FIX32
            tc.DefaultValue = FloatToFIX32(8.5R)
            tc.CurrentValue = tc.DefaultValue
            tc.SupportedOperations = TWQC.TWQC_ALLGET
            Caps.Add(tc.Capability, tc)

            tc = New TwainCapability
            tc.Capability = CAP.ICAP_PIXELFLAVOR
            tc.DataType = TWTY.TWTY_UINT16
            tc.EnumType = GetType(TWPF)
            tc.DefaultValue = TWPF.TWPF_CHOCOLATE
            tc.CurrentValue = tc.DefaultValue
            tc.SupportedOperations = TWQC.TWQC_ALLGET
            Caps.Add(tc.Capability, tc)

            tc = New TwainCapability
            tc.Capability = CAP.ICAP_PIXELTYPE
            tc.DataType = TWTY.TWTY_UINT16
            tc.EnumType = GetType(TWPT)
            tc.DefaultValue = TWPT.TWPT_RGB
            tc.CurrentValue = tc.DefaultValue
            tc.SupportedOperations = TWQC.TWQC_ALL
            Caps.Add(tc.Capability, tc)

            tc = New TwainCapability
            tc.Capability = CAP.ICAP_UNITS
            tc.DataType = TWTY.TWTY_UINT16
            tc.EnumType = GetType(TWUN)
            tc.DefaultValue = TWUN.TWUN_INCHES
            tc.CurrentValue = tc.DefaultValue
            tc.SupportedOperations = TWQC.TWQC_ALL
            Caps.Add(tc.Capability, tc)

            tc = New TwainCapability
            tc.Capability = CAP.ICAP_XFERMECH
            tc.DataType = TWTY.TWTY_UINT16
            tc.EnumType = GetType(TWSX)
            tc.DefaultValue = TWSX.TWSX_NATIVE
            tc.CurrentValue = tc.DefaultValue
            tc.SupportedOperations = TWQC.TWQC_ALL
            Caps.Add(tc.Capability, tc)

            tc = New TwainCapability
            tc.Capability = CAP.ICAP_XRESOLUTION
            tc.DataType = TWTY.TWTY_FIX32
            tc.DefaultValue = FloatToFIX32(150.0R)
            tc.CurrentValue = tc.DefaultValue
            tc.SupportedOperations = TWQC.TWQC_ALL
            Caps.Add(tc.Capability, tc)

            tc = New TwainCapability
            tc.Capability = CAP.ICAP_YRESOLUTION
            tc.DataType = TWTY.TWTY_FIX32
            tc.DefaultValue = FloatToFIX32(150.0R)
            tc.CurrentValue = tc.DefaultValue
            tc.SupportedOperations = TWQC.TWQC_ALL
            Caps.Add(tc.Capability, tc)

        End Sub
    End Class
End Namespace



