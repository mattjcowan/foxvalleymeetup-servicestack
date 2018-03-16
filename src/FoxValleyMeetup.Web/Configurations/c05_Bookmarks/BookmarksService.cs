// using System;
// using System.Collections.Generic;
// using System.IO;
// using System.Threading.Tasks;
// using ServiceStack;
// using ServiceStack.DataAnnotations;
// using ServiceStack.OrmLite;

// namespace FoxValleyMeetup.Web.Configurations.c05_Bookmarks
// {
//     public class BookmarksService : Service
//     {
//         public IAutoQueryDb AutoQuery { get; set; }

//         //Override with custom implementation
//         public object Any(FindBookmarks query)
//         {
//             var q = AutoQuery.CreateQuery(query, base.Request);
//             return AutoQuery.Execute(query, q);
//         }
//     }

//     [Authenticate]
//     public class BookmarksCrudService : Service
//     {
//         public async Task<object> Post(Bookmark request)
//         {
//             var user = GetSession(true);

//             Console.WriteLine("Files: " + Request.Files.Length);
//             if (Request.Files != null && Request.Files.Length > 0)
//             {
//                 Console.WriteLine("Uploading files");
//                 foreach(var file in Request.Files)
//                 {
//                     var bookmarks = BookmarkUtils.ToBookmarks(file.InputStream);
//                     foreach(var bookmark in bookmarks)
//                     {
//                         bookmark.CreatedBy = user.UserName;
//                         bookmark.CreatedById = user.Id.ToInt();
//                         bookmark.CreatedOn = DateTime.UtcNow;
//                         bookmark.ModifiedBy = user.UserName;
//                         bookmark.ModifiedById = user.Id.ToInt();
//                         bookmark.ModifiedOn = request.CreatedOn;
//                         try
//                         {
//                             await Db.InsertAsync(bookmark);
//                         }
//                         catch {}
//                     }
//                 }
//                 return null;
//             }

//             if (string.IsNullOrWhiteSpace(request.Url))
//                 throw new ArgumentNullException(nameof(request.Url));
//             if (string.IsNullOrWhiteSpace(request.Title))
//                 throw new ArgumentNullException(nameof(request.Title));

//             var id = await Db.InsertAsync(request, true);
//             return await Db.SingleByIdAsync<Bookmark>(id);
//         }

//         public async Task<object> Get(Bookmark request)
//         {
//             return await Db.SingleByIdAsync<Bookmark>(request.Id);
//         }
        
//         public async Task<object> Put(Bookmark request)
//         {
//             if (string.IsNullOrWhiteSpace(request.Url))
//                 throw new ArgumentNullException(nameof(request.Url));
//             if (string.IsNullOrWhiteSpace(request.Title))
//                 throw new ArgumentNullException(nameof(request.Title));

//             var existing = await Db.SingleByIdAsync<Bookmark>(request.Id);
//             if (existing == null)
//                 throw HttpError.NotFound("Not Found");

//             var user = GetSession(true);

//             request.ModifiedBy = user.UserName;
//             request.ModifiedById = user.Id.ToInt();
//             request.ModifiedOn = DateTime.UtcNow;

//             await Db.UpdateAsync(request);
//             return await Db.SingleByIdAsync<Bookmark>(request.Id);
//         }
        
//         public async Task<object> Patch(Bookmark request)
//         {
//             var existing = await Db.SingleByIdAsync<Bookmark>(request.Id);
//             if (existing == null)
//                 throw HttpError.NotFound("Not Found");

//             existing.PopulateWithNonDefaultValues(request);

//             var user = GetSession(true);

//             existing.ModifiedBy = user.UserName;
//             existing.ModifiedById = user.Id.ToInt();
//             existing.ModifiedOn = DateTime.UtcNow;

//             await Db.UpdateAsync(existing);
//             return await Db.SingleByIdAsync<Bookmark>(request.Id);
//         }

//         public async Task<object> Delete(Bookmark request)
//         {
//             var existing = await Db.SingleByIdAsync<Bookmark>(request.Id);
//             if (existing == null)
//                 throw HttpError.NotFound("Not Found");

//             await Db.DeleteAsync(request);
//             return await Db.SingleByIdAsync<Bookmark>(request.Id);
//         }
//     }

//     public static class BookmarkUtils
//     {
//         // for now assumes Url,Title,Description,Category,Tags
//         public static List<Bookmark> ToBookmarks(this Stream stream)
//         {
//             var bookmarks = new List<Bookmark>();
//             var firstLine = true;
//             foreach (var line in stream.ReadLines())
//             {
//                 if (firstLine)
//                 {
//                     firstLine = false;
//                     continue;
//                 }
                
//                 var items = line.Split(',');
//                 var bookmark = new Bookmark();
//                 if (items.Length > 0) bookmark.Url = items[0];
//                 if (items.Length > 1) bookmark.Title = items[1];
//                 if (items.Length > 2) bookmark.Description = items[2];
//                 if (items.Length > 3) bookmark.Category = items[3];
//                 if (!string.IsNullOrWhiteSpace(bookmark.Url) && !string.IsNullOrWhiteSpace(bookmark.Title))
//                     bookmarks.Add(bookmark);
//             }
//             return bookmarks;
//         }

//     }

//     [Route("/bookmarks", "GET")]
//     public class FindBookmarks : QueryDb<Bookmark>
//     {
//         public string[] Ratings { get; set; }
//     }

//     [Route("/bookmarks", "POST")]
//     [Route("/bookmarks/{Id}", "GET,PUT,PATCH,DELETE")]
//     [Alias("Bookmarks")]
//     public class Bookmark: IReturn<Bookmark>
//     {
//         [PrimaryKey, AutoIncrement]
//         public int Id { get; set; }
//         [Required, StringLength(256)]
//         public string Url { get; set; }
//         [Required, StringLength(256)]
//         public string Title { get; set; }
//         public string Description { get; set; }
//         public string Category { get; set; }
//         public string[] Tags { get; set; }
//         public DateTime? CreatedOn { get; set; }
//         public string CreatedBy { get; set; }
//         public int? CreatedById { get; set; }
//         public DateTime? ModifiedOn { get; set; }
//         public string ModifiedBy { get; set; }
//         public int? ModifiedById { get; set; }
//     }
// }