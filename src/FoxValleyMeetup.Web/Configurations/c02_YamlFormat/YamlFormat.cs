// using System;
// using System.IO;
// using ServiceStack;
// using ServiceStack.Web;

// namespace FoxValleyMeetup.Web.Configurations.c02_YamlFormat
// {
//     public class YamlFormat : IPlugin
//     {
//         public YamlFormat(bool usePlainText = false, bool useRequestTypeAsName = false)
//         {
//             UsePlainText = usePlainText;
//             UseRequestTypeAsName = useRequestTypeAsName;
//         }

//         public bool UsePlainText { get; }
//         public bool UseRequestTypeAsName { get; }

//         public void Register(IAppHost appHost)
//         {
//             //Register the 'application/yaml' content-type and serializers (format is inferred from the last part of the content-type)
//             appHost.ContentTypes.Register(MimeTypes.Yaml, YamlSerializer.SerializeToStream, YamlSerializer.DeserializeFromStream);

//             //Add a response filter to add a 'Content-Disposition' header so browsers treat it natively as a .yaml file
//             appHost.GlobalResponseFilters.Add((req, res, dto) =>
//             {
//                 if (req.ResponseContentType == MimeTypes.Yaml)
//                 {
//                     if (UsePlainText)
//                     {
//                         res.RemoveHeader("Content-Type");
//                         res.AddHeader("Content-Type", MimeTypes.PlainText);
//                     }
//                     else
//                     {
//                         if (UseRequestTypeAsName)
//                         {
//                             res.AddHeader(HttpHeaders.ContentDisposition, $"attachment;filename={req.OperationName}.yaml");
//                         }
//                     }
//                 }
//             });
//         }
//     }

//     public class YamlSerializer
//     {
//         public static object DeserializeFromStream(Type type, Stream fromStream)
//         {
//             using(var reader = new StreamReader(fromStream))
//             {
//                 return YamlSerializer.SerializeFromReader(type, reader);
//             }
//         }

//         private static object SerializeFromReader(Type type, StreamReader reader)
//         {
//             var serializer = new YamlDotNet.Serialization.Deserializer();
//             return serializer.Deserialize(reader, type);
//         }

//         public static void SerializeToStream(IRequest requestContext, object request, Stream stream)
//         {
//             YamlSerializer.SerializeToStream(request, stream);
//         }

//         public static void SerializeToStream(object request, Stream stream)
//         {
//             using(var writer = new StreamWriter(stream))
//             {
//                 YamlSerializer.SerializeToWriter(request, writer);
//             }
//         }

//         private static void SerializeToWriter(object request, StreamWriter writer)
//         {
//             var serializer = new YamlDotNet.Serialization.Serializer();
//             serializer.Serialize(writer, request);
//         }
//     }
// }