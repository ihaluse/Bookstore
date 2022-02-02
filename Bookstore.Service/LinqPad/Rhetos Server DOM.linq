<Query Kind="Program">
  <Reference Relative="..\bin\Autofac.dll">C:\Users\ihalusek\source\repos\Bookstore\Bookstore.Service\bin\Autofac.dll</Reference>
  <Reference Relative="..\bin\EntityFramework.dll">C:\Users\ihalusek\source\repos\Bookstore\Bookstore.Service\bin\EntityFramework.dll</Reference>
  <Reference Relative="..\bin\EntityFramework.SqlServer.dll">C:\Users\ihalusek\source\repos\Bookstore\Bookstore.Service\bin\EntityFramework.SqlServer.dll</Reference>
  <Reference Relative="..\bin\Bookstore.Service.dll">C:\Users\ihalusek\source\repos\Bookstore\Bookstore.Service\bin\Bookstore.Service.dll</Reference>
  <Reference>..\bin\Generated\ServerDom.Orm.dll</Reference>
  <Reference>..\bin\Generated\ServerDom.Repositories.dll</Reference>
  <Reference Relative="..\bin\NLog.dll">C:\Users\ihalusek\source\repos\Bookstore\Bookstore.Service\bin\NLog.dll</Reference>
  <Reference Relative="..\bin\Oracle.ManagedDataAccess.dll">C:\Users\ihalusek\source\repos\Bookstore\Bookstore.Service\bin\Oracle.ManagedDataAccess.dll</Reference>
  <Reference>..\bin\Rhetos.AspNetFormsAuth.dll</Reference>
  <Reference Relative="..\bin\Rhetos.Dom.DefaultConcepts.dll">C:\Users\ihalusek\source\repos\Bookstore\Bookstore.Service\bin\Rhetos.Dom.DefaultConcepts.dll</Reference>
  <Reference Relative="..\bin\Rhetos.Dom.DefaultConcepts.Interfaces.dll">C:\Users\ihalusek\source\repos\Bookstore\Bookstore.Service\bin\Rhetos.Dom.DefaultConcepts.Interfaces.dll</Reference>
  <Reference Relative="..\bin\Rhetos.Dsl.DefaultConcepts.dll">C:\Users\ihalusek\source\repos\Bookstore\Bookstore.Service\bin\Rhetos.Dsl.DefaultConcepts.dll</Reference>
  <Reference Relative="..\bin\Rhetos.Processing.DefaultCommands.Interfaces.dll">C:\Users\ihalusek\source\repos\Bookstore\Bookstore.Service\bin\Rhetos.Processing.DefaultCommands.Interfaces.dll</Reference>
  <Reference Relative="..\bin\Rhetos.Configuration.Autofac.dll">C:\Users\ihalusek\source\repos\Bookstore\Bookstore.Service\bin\Rhetos.Configuration.Autofac.dll</Reference>
  <Reference Relative="..\bin\Rhetos.Dom.Interfaces.dll">C:\Users\ihalusek\source\repos\Bookstore\Bookstore.Service\bin\Rhetos.Dom.Interfaces.dll</Reference>
  <Reference Relative="..\bin\Rhetos.Dsl.Interfaces.dll">C:\Users\ihalusek\source\repos\Bookstore\Bookstore.Service\bin\Rhetos.Dsl.Interfaces.dll</Reference>
  <Reference Relative="..\bin\Rhetos.Interfaces.dll">C:\Users\ihalusek\source\repos\Bookstore\Bookstore.Service\bin\Rhetos.Interfaces.dll</Reference>
  <Reference Relative="..\bin\Rhetos.Logging.Interfaces.dll">C:\Users\ihalusek\source\repos\Bookstore\Bookstore.Service\bin\Rhetos.Logging.Interfaces.dll</Reference>
  <Reference Relative="..\bin\Rhetos.Persistence.Interfaces.dll">C:\Users\ihalusek\source\repos\Bookstore\Bookstore.Service\bin\Rhetos.Persistence.Interfaces.dll</Reference>
  <Reference Relative="..\bin\Rhetos.Processing.Interfaces.dll">C:\Users\ihalusek\source\repos\Bookstore\Bookstore.Service\bin\Rhetos.Processing.Interfaces.dll</Reference>
  <Reference Relative="..\bin\Rhetos.Security.Interfaces.dll">C:\Users\ihalusek\source\repos\Bookstore\Bookstore.Service\bin\Rhetos.Security.Interfaces.dll</Reference>
  <Reference Relative="..\bin\Rhetos.TestCommon.dll">C:\Users\ihalusek\source\repos\Bookstore\Bookstore.Service\bin\Rhetos.TestCommon.dll</Reference>
  <Reference Relative="..\bin\Rhetos.Utilities.dll">C:\Users\ihalusek\source\repos\Bookstore\Bookstore.Service\bin\Rhetos.Utilities.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.DirectoryServices.AccountManagement.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.DirectoryServices.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Runtime.Serialization.dll</Reference>
  <Namespace>Oracle.ManagedDataAccess.Client</Namespace>
  <Namespace>Rhetos.Configuration.Autofac</Namespace>
  <Namespace>Rhetos.Dom</Namespace>
  <Namespace>Rhetos.Dom.DefaultConcepts</Namespace>
  <Namespace>Rhetos.Dsl</Namespace>
  <Namespace>Rhetos.Dsl.DefaultConcepts</Namespace>
  <Namespace>Rhetos.Logging</Namespace>
  <Namespace>Rhetos.Persistence</Namespace>
  <Namespace>Rhetos.Security</Namespace>
  <Namespace>Rhetos.Utilities</Namespace>
  <Namespace>System</Namespace>
  <Namespace>System.Collections.Generic</Namespace>
  <Namespace>System.Data.Entity</Namespace>
  <Namespace>System.DirectoryServices</Namespace>
  <Namespace>System.DirectoryServices.AccountManagement</Namespace>
  <Namespace>System.IO</Namespace>
  <Namespace>System.Linq</Namespace>
  <Namespace>System.Reflection</Namespace>
  <Namespace>System.Runtime.Serialization.Json</Namespace>
  <Namespace>System.Text</Namespace>
  <Namespace>System.Xml</Namespace>
  <Namespace>System.Xml.Serialization</Namespace>
  <Namespace>Autofac</Namespace>
  <Namespace>Rhetos.TestCommon</Namespace>
  <Namespace>Rhetos</Namespace>
</Query>

void Main()
{
	string applicationFolder = Path.GetDirectoryName(Util.CurrentQueryPath);
	ConsoleLogger.MinLevel = EventType.Info; // Use EventType.Trace for more detailed log.
	
	using (var scope = ProcessContainer.CreateScope(applicationFolder))
    {
        var context = scope.Resolve<Common.ExecutionContext>();
        var repository = context.Repository;
		
		//BOOK TITLE + NAME OF AUTHOR
		var allAuthors = repository.Bookstore.Person.Load();

		var allBooks = repository.Bookstore.Book.Load().Join(allAuthors, book => book.AuthorID, author => author.ID,
		(book, author) => new {book.Title, author.Name});
		//allBooks.Dump();

		//NUMBER OF TOPICS + NAME OF AUTHOR
		var numberOfTopicsAndAuthor = repository.Bookstore.Book.Query().Select(n => new { n.Author.Name, n.Extension_BookInfo.NumberOfTopics });
		//numberOfTopicsAndAuthor.Dump();

		//SQL QUERY FOR UPPER LINQ
		numberOfTopicsAndAuthor.ToString().Dump("Generated SQL (numberOfTopicsAndAuthor)");

		//INSERT MANY BOOKS
		var actionParameter = new Bookstore.InsertManyBooks
		{
			NumberOfBooks = 2,
			TitlePrefix = "New Book"
		};
		repository.Bookstore.InsertManyBooks.Execute(actionParameter);
		//scope.CommitAndClose()
	}
}