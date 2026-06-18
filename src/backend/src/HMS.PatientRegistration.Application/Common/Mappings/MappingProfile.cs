using AutoMapper;
using HMS.PatientRegistration.Application.MasterData.DTOs;
using HMS.PatientRegistration.Application.Patients.DTOs;
using HMS.PatientRegistration.Domain.Entities;

namespace HMS.PatientRegistration.Application.Common.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Patient, PatientDto>()
            .ForMember(d => d.Gender, o => o.MapFrom(s => s.Gender.ToString()))
            .ForMember(d => d.BloodGroup, o => o.MapFrom(s => s.BloodGroup.ToString()))
            .ForMember(d => d.Status, o => o.MapFrom(s => s.Status.ToString()))
            .ForMember(d => d.FullName, o => o.MapFrom(s => s.FullName));
        CreateMap<Patient, PatientSummaryDto>()
            .ForMember(d => d.Gender, o => o.MapFrom(s => s.Gender.ToString()))
            .ForMember(d => d.Status, o => o.MapFrom(s => s.Status.ToString()))
            .ForMember(d => d.FullName, o => o.MapFrom(s => s.FullName))
            .ForMember(d => d.PrimaryPhone, o => o.MapFrom(s => PatientMappingHelper.GetPrimaryPhone(s)));
        CreateMap<PatientAddress, PatientAddressDto>()
            .ForMember(d => d.AddressType, o => o.MapFrom(s => s.AddressType.ToString()));
        CreateMap<PatientContact, PatientContactDto>()
            .ForMember(d => d.ContactType, o => o.MapFrom(s => s.ContactType.ToString()));
        CreateMap<PatientInsurance, PatientInsuranceDto>();
        CreateMap<EmergencyContact, EmergencyContactDto>();
        CreateMap<MasterDataItem, MasterDataItemDto>()
            .ForMember(d => d.Type, o => o.MapFrom(s => s.Type.ToString()));

        CreateMap<CreatePatientAddressDto, PatientAddress>();
        CreateMap<CreatePatientContactDto, PatientContact>();
        CreateMap<CreatePatientInsuranceDto, PatientInsurance>();
        CreateMap<CreateEmergencyContactDto, EmergencyContact>();

        CreateMap<UpdatePatientAddressDto, PatientAddress>();
        CreateMap<UpdatePatientContactDto, PatientContact>();
        CreateMap<UpdatePatientInsuranceDto, PatientInsurance>();
        CreateMap<UpdateEmergencyContactDto, EmergencyContact>();
    }
}
