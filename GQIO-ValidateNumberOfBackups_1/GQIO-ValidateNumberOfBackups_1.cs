using System;
using Skyline.DataMiner.Analytics.GenericInterface;

[GQIMetaData(Name = "ValidateNumberOfBacktups")]
public class ValidateNumberOfBacktups : IGQIRowOperator, IGQIInputArguments, IGQIColumnOperator
{
    private readonly GQIStringDropdownArgument _backupPatternArg = new GQIStringDropdownArgument("Backup Pattern", new string[] { "Daily"/*, "Weekly"*/ }) { IsRequired = true };
    private readonly GQIDateTimeArgument _startArg = new GQIDateTimeArgument("Start Time") { IsRequired = true };
    private readonly GQIDateTimeArgument _endArg = new GQIDateTimeArgument("End Time") { IsRequired = true };
    private readonly GQIIntArgument _numberOfBackupsArg = new GQIIntArgument("Amount of backups that happened") { IsRequired = true };

    private readonly GQIStringColumn _validationColumn = new GQIStringColumn("Validation");

    private String _backupPattern;

    private int _numberOfBackups;
    private int _numberOfBackupsExpected;

    public GQIArgument[] GetInputArguments()
    {
        return new GQIArgument[] { _backupPatternArg, _startArg, _endArg, _numberOfBackupsArg };
    }

    public OnArgumentsProcessedOutputArgs OnArgumentsProcessed(OnArgumentsProcessedInputArgs args)
    {
        _backupPattern = args.GetArgumentValue(_backupPatternArg);
        DateTime start = args.GetArgumentValue(_startArg);
        DateTime end = args.GetArgumentValue(_endArg);
        _numberOfBackups = args.GetArgumentValue(_numberOfBackupsArg);
        _numberOfBackupsExpected = CalculateNumberOfDaysExpected(start, end);
        return new OnArgumentsProcessedOutputArgs();
    }

    public void HandleColumns(GQIEditableHeader header)
    {
        header.AddColumns(_validationColumn);
    }

    public void HandleRow(GQIEditableRow row)
    {
        try
        {
            String validation = (_numberOfBackups == _numberOfBackupsExpected) ? "OK" : "not OK";

            row.SetValue(_validationColumn, validation);
        }
        catch (Exception)
        {
        }
    }

    private static int CalculateNumberOfDaysExpected(DateTime start, DateTime end)
    {
        // Currently limited to Daily patttern !!!
        // Calculate the difference in days
        TimeSpan difference = end - start;
        return (int)difference.TotalDays;
    }
}