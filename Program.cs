using ClosedXML.Excel;
using System.Data;
using System.IO.Compression;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;


namespace CommandPattern;


#nullable disable
// Nuget: ClosedXML




public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
}








// Receiver
public class ExcelFile<T>
{
    private readonly List<T> _list;
    public string FileName => $"{typeof(T).Name}.xlsx";


    public ExcelFile(List<T> list)
    {
        _list = list;
    }

    public MemoryStream Create()
    {
        var wb = new XLWorkbook();
        var ds = new DataSet();

        ds.Tables.Add(GetTable());
        wb.Worksheets.Add(ds);

        var excelMemory = new MemoryStream();
        wb.SaveAs(excelMemory);

        return excelMemory;
    }


    private DataTable GetTable()
    {
        var table = new DataTable();

        var type = typeof(T);

        type.GetProperties()
            .ToList()
            .ForEach(x => table.Columns.Add(x.Name, x.PropertyType));


        _list.ForEach(x =>
        {
            var values = type.GetProperties()
                                .Select(properyInfo => properyInfo
                                .GetValue(x, null))
                                .ToArray();

            table.Rows.Add(values);
        });

        return table;
    }
}







// Homework


public interface ITableActionCommand
{
    void Execute();
}



public class CreateExcelTableActionCommand<T> : ITableActionCommand
{
    private readonly ExcelFile<T> _excelFile;

    public CreateExcelTableActionCommand(ExcelFile<T> excelFile)
        => _excelFile = excelFile;


    public void Execute()
    {
        MemoryStream excelMemoryStream = _excelFile.Create();
        File.WriteAllBytes(_excelFile.FileName, excelMemoryStream.ToArray());
    }
}




//// Homework
// public class CreatePdfTableActionCommand<T> : ITableActionCommand












// Invoker
class FileCreateInvoker
{
    private ITableActionCommand _tableActionCommand;
    private List<ITableActionCommand> tableActionCommands = new List<ITableActionCommand>();

    public void SetCommand(ITableActionCommand tableActionCommand)
    {
        _tableActionCommand = tableActionCommand;
    }

    public void AddCommand(ITableActionCommand tableActionCommand)
    {
        tableActionCommands.Add(tableActionCommand);
    }

    public void CreateFile()
    {
        _tableActionCommand.Execute();
    }

    public void CreateFiles()
    {
        throw new NotImplementedException();
        // ZipArchive
    }
}


//////////===============PDF ===============================////////////////
public interface IPdfCommand
{
    void Execute(Document document);
}

public class PdfWriteTextCommand : IPdfCommand
{
    private string text;

    public PdfWriteTextCommand(string text)
    {
        this.text = text;
    }

    public void Execute(Document document)
    {
        document.Add(new Paragraph(text));
    }
}


public class PdfFileInvoker
{
    private Document document;
    private IPdfCommand command;

    public PdfFileInvoker(Document document)
    {
        this.document = document;
    }

    public void SetCommand(IPdfCommand command)
    {
        this.command = command;
    }

    public void ExecuteCommand()
    {
        if (command != null)
        {
            command.Execute(document);
        }
    }

    public MemoryStream Create()
    {
        throw new NotImplementedException();
    }
}






//////////////////////////////////////////////////////////////////


// Command interface


// Concrete Command for writing text to the PDF


// Invoker class




class Program
{
    static void Main()
    {

        //var products = Enumerable.Range(1, 30).Select(index =>
        //    new Product
        //    {
        //        Id = index,
        //        Name = $"Product {index}",
        //        Price = index + 100,
        //        Stock = index
        //    }
        //).ToList();


        //ExcelFile<Product> receiver = new(products);


        //FileCreateInvoker invoker = new();
        //invoker.SetCommand(new CreateExcelTableActionCommand<Product>(receiver));
        //invoker.CreateFile();
        ////////////////////////////////////////////////////////////////////////////////
        ///


        
        
            // Create a new PDF document
            Document document = new Document();
            PdfWriter.GetInstance(document, new FileStream("New.pdf", FileMode.Create));
            document.Open();

        // Create the invoker
        PdfFileInvoker invoker = new PdfFileInvoker(document);

            // Add different commands
            invoker.SetCommand(new PdfWriteTextCommand("Slithern"));
            invoker.ExecuteCommand();

            invoker.SetCommand(new PdfWriteTextCommand("Command Design Pattern in C#"));
            invoker.ExecuteCommand();

            // Fayli Save edib baglamag
            document.Close();

            Console.WriteLine("PDF created successfully!");
            Console.ReadLine();
       

}
}