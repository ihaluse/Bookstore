using Autofac;
using Bookstore.Service.Test.Tools;
using Common.Queryable;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhetos;
using Rhetos.Dom.DefaultConcepts;
using Rhetos.TestCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bookstore.Service.Test
{
    [TestClass]
    public class BooksTest
    {
        public Bookstore_Book CreateNewBook(string title = null)
        {
            if (title is null) title = Guid.NewGuid().ToString();
            return new Bookstore_Book { Title = title };
        }

        public List<Comment> CreateNewCommentsForBook(Book book, int numberOfCommets = 5)
        {
            var comments = new List<Comment>();
            for (var i = 0; i < numberOfCommets; i++)
            {
                comments.Add(new Comment() { BookID = book.ID, Text = "Comment " + i.ToString() });
            }

            return comments;
        }

        [TestMethod]
        public void AutomaticallyUpdateNumberOfComments()
        {
            using (var scope = TestScope.Create())
            {
                var repository = scope.Resolve<Common.DomRepository>();

                var book = CreateNewBook();
                repository.Bookstore.Book.Insert(book);

                int? readNumberOfComments() => repository.Bookstore.BookInfo_Computed
                    .Query(bi => bi.ID == book.ID)
                    .Select(bi => bi.NumberOfComments)
                    .Single();

                Assert.AreEqual(0, readNumberOfComments());

                var firstComment = CreateNewCommentsForBook(book, 1);
                repository.Bookstore.Comment.Insert(firstComment);
                Assert.AreEqual(1, readNumberOfComments());

                repository.Bookstore.Comment.Insert(CreateNewCommentsForBook(book, 2));
                Assert.AreEqual(3, readNumberOfComments());

                repository.Bookstore.Comment.Delete(firstComment);
                Assert.AreEqual(2, readNumberOfComments());
            }
        }

        [TestMethod]
        public void CommonMisspellingValidation()
        {
            using (var scope = TestScope.Create())
            {
                var repository = scope.Resolve<Common.DomRepository>();

                var books = new[]
                {
                    CreateNewBook("spirit"),
                    CreateNewBook("opportunity"),
                    CreateNewBook("curiousity"),
                    CreateNewBook("curiousity2")
                }.AsQueryable();

                var invalidBooks = repository.Bookstore.Book.Filter(books, new CommonMisspelling());

                Assert.AreEqual("curiousity, curiousity2", TestUtility.DumpSorted(invalidBooks, book => book.Title)); 
            }
        }

        [TestMethod]
        public void LongBooksValidation()
        {
            using (var scope = TestScope.Create())
            {
                var repository = scope.Resolve<Common.DomRepository>();

                var books = new[]
                {
                    new Bookstore_Book { NumberOfPages = 100, Title = Guid.NewGuid().ToString() },
                    new Bookstore_Book { NumberOfPages = 500, Title = Guid.NewGuid().ToString() },
                    new Bookstore_Book { NumberOfPages = 499, Title = Guid.NewGuid().ToString() },
                    new Bookstore_Book { NumberOfPages = 1000, Title = Guid.NewGuid().ToString() }
                }.AsQueryable();

                var longBooks = repository.Bookstore.Book.Filter(books, new LongBooks());

                Assert.AreEqual(2, longBooks.Count());
            }
        }

        [TestMethod]
        public void TotalBooksByAuthorValidation()
        {
            using (var scope = TestScope.Create())
            {
                var repository = scope.Resolve<Common.DomRepository>();

                var newAuthor = new Bookstore_Person { Name = "Author1" };
                var newAuthor2 = new Bookstore_Person { Name = "Author2" };
                var newAuthor3 = new Bookstore_Person { Name = "Author3" };
                repository.Bookstore.Person.Insert(newAuthor, newAuthor2, newAuthor3);

                var bookToRemove = new Bookstore_Book { Title = Guid.NewGuid().ToString(), AuthorID = newAuthor.ID };
                repository.Bookstore.Book.Insert(
                    new Bookstore_Book { Title = Guid.NewGuid().ToString(), AuthorID = newAuthor.ID },
                    new Bookstore_Book { Title = Guid.NewGuid().ToString(), AuthorID = newAuthor2.ID },
                    bookToRemove
                );
                var totalBooksByAuthorQuery = repository.Bookstore.TotalBooksByAuthor.Query();

                var booksByFirstAuthor = totalBooksByAuthorQuery.Where(n => n.ID == newAuthor.ID).Select(n => n.NumberOfBooks).FirstOrDefault();
                var booksByThirdAuthor = totalBooksByAuthorQuery.Where(n => n.ID == newAuthor3.ID).Select(n => n.NumberOfBooks).FirstOrDefault();

                Assert.AreEqual(2, booksByFirstAuthor);
                Assert.AreEqual(0, booksByThirdAuthor);

                repository.Bookstore.Book.Delete(bookToRemove);
                booksByFirstAuthor = totalBooksByAuthorQuery.Where(n => n.ID == newAuthor.ID).Select(n => n.NumberOfBooks).FirstOrDefault();
                Assert.AreEqual(1, booksByFirstAuthor);
            }
        }

        [TestMethod]
        public void ParallelCodeGeneration()
        {
            DeleteUnitTestBooks();

            var books = new[]
            {
                new Book { Code = $"{UnitTestBookCodePrefix}+++", Title = Guid.NewGuid().ToString() },
                new Book { Code = $"{UnitTestBookCodePrefix}+++", Title = Guid.NewGuid().ToString() },
                new Book { Code = $"{UnitTestBookCodePrefix}ABC+", Title = Guid.NewGuid().ToString() },
                new Book { Code = $"{UnitTestBookCodePrefix}ABC+", Title = Guid.NewGuid().ToString() }
            };

            for (int retry = 0; retry < 3; retry++)
            {
                Parallel.ForEach(books, book =>
                {
                    using (var scope = TestScope.Create())
                    {
                        var repository = scope.Resolve<Common.DomRepository>();
                        repository.Bookstore.Book.Insert(book);
                        scope.CommitAndClose();
                    }
                });

                using (var scope = TestScope.Create())
                {
                    var repository = scope.Resolve<Common.DomRepository>();
                    var booksFromDb = repository.Bookstore.Book.Load(book => book.Code.StartsWith(UnitTestBookCodePrefix));
                    Assert.AreEqual(
                        //$"{UnitTestBookCodePrefix}000, {UnitTestBookCodePrefix}002, {UnitTestBookCodePrefix}ABC1, {UnitTestBookCodePrefix}ABC2",
                        $"{UnitTestBookCodePrefix}001, {UnitTestBookCodePrefix}002, {UnitTestBookCodePrefix}ABC1, {UnitTestBookCodePrefix}ABC2",
                        TestUtility.DumpSorted(booksFromDb, book => book.Code));
                }

                DeleteUnitTestBooks();
            }
        }

        private const string UnitTestBookCodePrefix = "UniTestBooksCodePrefix";

        private void DeleteUnitTestBooks()
        {
            using (var scope = TestScope.Create())
            {
                var repository = scope.Resolve<Common.DomRepository>();
                var testBooks = repository.Bookstore.Book.Load(book => book.Code.StartsWith(UnitTestBookCodePrefix));
                repository.Bookstore.Book.Delete(testBooks);
                scope.CommitAndClose();
            }
        }
    }
}
