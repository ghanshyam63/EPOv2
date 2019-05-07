namespace DomainModel.Enums
{
    public enum TileSubType
    {
        OTA = 0,//Orders to Approve
        OTM = 1,//Orders to be Matched
        MO = 2,//My Orders
        ITA = 3,//Invoice to Approve
        CTA = 4,//Capex to Approve
        EE = 10,//Employee Exceptions
        YCSL = 20,//Yesterday Casefill
        YATP = 30, //Yesterday ATP
        YRFT = 40, //Yesterday RFT
        YYCOS = 50,//Yesterday Yiels COS
        YYSPIN = 51,//Yesterday Yield SPIN
        SAY = 60, //Safety Actions for Employee
        STIN = 61, //Safety Incidents Table
        STSWAT =62,//Safety Safety actions Walk & Talk Table
        STSAR =63,//Safety Safety actions register Table
        YLCOS=70,//Yield Line Graph COS
    }
}