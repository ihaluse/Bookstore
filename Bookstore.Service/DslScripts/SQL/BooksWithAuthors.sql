SELECT 
	book.Title,
	author.name
FROM 
  [Test].[Bookstore].[Book] book
  left join Test.Bookstore.Person author on author.ID = book.AuthorID