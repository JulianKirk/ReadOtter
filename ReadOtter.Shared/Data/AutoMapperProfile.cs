using ReadOtter.Shared.Data.Models;
using VersOne.Epub;
using VersOne.Epub.Schema;

namespace ReadOtter.Shared.Data
{
    public class AutoMapperProfile : AutoMapper.Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<EpubMetadata, BookMetaData>();
            CreateMap<EpubLocalTextContentFile, ContentChapter>()
                .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content))
                .ForMember(dest => dest.Title, opt => opt.Ignore())
                .ForMember(dest => dest.Key, opt => opt.MapFrom(src => src.Key))
                .ForMember(dest => dest.Index, opt => opt.Ignore());
            CreateMap<EpubLocalTextContentFileRef, ContentChapter>()
                .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.ReadContent()))
                .ForMember(dest => dest.Title, opt => opt.Ignore())
                .ForMember(dest => dest.Key, opt => opt.MapFrom(src => src.Key))
                .ForMember(dest => dest.Index, opt => opt.Ignore());
        }
    }
}
