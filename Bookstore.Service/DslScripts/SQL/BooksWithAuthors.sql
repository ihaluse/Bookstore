SELECT 
	book.Title,
	author.name
FROM 
  Test.Bookstore.Person author
  left join [Test].[Bookstore].[Book] book  on author.ID = book.AuthorID