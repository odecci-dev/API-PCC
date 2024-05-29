using API_PCC.ApplicationModels;
using API_PCC.Models;
using static API_PCC.Controllers.BuffAnimalsController;

namespace API_PCC.Utils
{
    public class QueryBuilder
    {

        public static String buildHerdSearchQuery(BuffHerdSearchFilterModel searchFilterModel)
        {
            String herdSelect = Constants.DBQuery.HERD_SELECT + " WHERE H_BUFF_HERD.DELETE_FLAG = 0 ";
            if (searchFilterModel.searchValue != null && searchFilterModel.searchValue != "")
            {
                herdSelect = herdSelect + "AND (H_BUFF_HERD.HERD_CODE LIKE '%' + @SearchParam + '%' OR H_BUFF_HERD.HERD_NAME LIKE '%' + @SearchParam +'%') ";
            }

            if (searchFilterModel.filterBy != null)
            {
                if (searchFilterModel.filterBy.BreedTypeCode != null && searchFilterModel.filterBy.BreedTypeCode != "")
                {
                    herdSelect = herdSelect + "AND H_BUFF_HERD.BREED_TYPE_CODE = @BreedTypeCode ";
                }

                if (searchFilterModel.filterBy.HerdClassDesc != null && searchFilterModel.filterBy.HerdClassDesc != "")
                {
                    herdSelect = herdSelect + "AND H_Herd_Classification.HERD_CLASS_DESC = @HerdClassDesc ";
                }

                if (searchFilterModel.filterBy.feedingSystemCode != null && searchFilterModel.filterBy.feedingSystemCode != "")
                {
                    herdSelect = herdSelect + "AND H_BUFF_HERD.FEEDING_SYSTEM_CODE = @FeedingSystemCode  ";
                }
            }

            if (searchFilterModel.dateFrom != null && searchFilterModel.dateFrom != "")
            {
                herdSelect = herdSelect + "AND H_BUFF_HERD.Date_Created >= @DateFrom ";
            }

            if (searchFilterModel.dateTo != null && searchFilterModel.dateTo != "")
            {
                herdSelect = herdSelect + "AND H_BUFF_HERD.Date_Created >= @DateFrom ";
            }

            return herdSelect;
        }

        public static String buildHerdOwnerJoinQuery(String herdCode)
        {
            return "SELECT FA.* FROM H_BUFF_HERD BH INNER JOIN TBL_FARMOWNER FA ON BH.OWNER = FA.ID where BH.HERD_CODE = '" + herdCode + "'";
        }
        public static String buildHerdViewQuery()
        {
            String herdSelect = Constants.DBQuery.HERD_SELECT + "WHERE H_Buff_Herd.DELETE_FLAG = 0 AND HERD_CODE = @HerdCode";
            return herdSelect;
        }
        public static String buildHerdSearchByHerdCode()
        {
            return Constants.DBQuery.HERD_SELECT + "WHERE DELETE_FLAG = 0 AND HERD_CODE = @HerdCode";
        }
        public static String buildHerdSearchAll()
        {
            return Constants.DBQuery.HERD_SELECT + "WHERE DELETE_FLAG = 0 ";
        }

        public static String buildHerdArchiveQuery()
        {
            return Constants.DBQuery.HERD_SELECT + "WHERE H_Buff_Herd.DELETE_FLAG = 1";
        }
        public static String buildHerdSelectForRestoreQuery()
        {
            return Constants.DBQuery.HERD_SELECT + "WHERE H_Buff_Herd.DELETE_FLAG = 1 AND id = @Id";
        }
        public static String buildHerdDuplicateCheckSaveQuery()
        {
            return Constants.DBQuery.HERD_SELECT + "WHERE DELETE_FLAG = 0 AND HERD_NAME = @HerdName AND HERD_CODE = @HerdCode";
        }

        public static String buildHerdSelectQueryById()
        {
            return Constants.DBQuery.HERD_SELECT + "WHERE H_Buff_Herd.DELETE_FLAG = 0 AND H_Buff_Herd.id = @Id";
        }
        public static String buildHerdSelectDuplicateQueryByIdHerdNameHerdCode()
        {
            return Constants.DBQuery.HERD_SELECT + "WHERE H_Buff_Herd.DELETE_FLAG = 0 AND NOT H_Buff_Herd.id = @Id  AND HERD_NAME = @HerdName AND HERD_CODE = @HerdCode";
        }

        public static String buildHerdSelectQueryByHerdClassDesc()
        {
            return Constants.DBQuery.HERD_SELECT + "WHERE H_Buff_Herd.DELETE_FLAG = 0 AND HERD_CLASS_DESC = @HerdClassDesc";
        }

        public static String buildFarmOwnerSearchQueryByFirstNameAndLastName()
        {
            return Constants.DBQuery.FARM_OWNER_SELECT + "WHERE FirstName = @FirstName AND LastName = @LastName";
        }

        public static String buildFarmOwnerSearchQueryByFirstNameOrLastName()
        {
            return Constants.DBQuery.FARM_OWNER_SELECT + "WHERE FirstName = @SearchParam OR LastName = @SearchParam";
        }

        public static String buildFarmOwnerSearchQueryById()
        {
            return Constants.DBQuery.FARM_OWNER_SELECT + "WHERE id = @Id";
        }

        public static String buildBuffAnimalSearch(BuffAnimalSearchFilterModel searchFilterModel)
        {
            String buffAnimalSelect = Constants.DBQuery.BUFF_ANIMAL_SELECT + "WHERE DELETE_FLAG = 0 ";
            if (searchFilterModel.searchValue != null && searchFilterModel.searchValue != "")
            {
                buffAnimalSelect = buffAnimalSelect + "AND (ANIMAL_ID_NUMBER LIKE '%' + @SearchParam + '%' OR ANIMAL_NAME LIKE '%' + @SearchParam + '%') ";
            }

            if (searchFilterModel.sex != null && searchFilterModel.sex != "")
            {
                buffAnimalSelect = buffAnimalSelect + "AND SEX = @Sex ";
            }

            if (searchFilterModel.status != null && searchFilterModel.status != "")
            {
                buffAnimalSelect = buffAnimalSelect + "AND STATUS = @Status ";
            }

            if (searchFilterModel.filterBy != null)
            {
                if (searchFilterModel.filterBy.BloodCode != null && searchFilterModel.filterBy.BloodCode != "")
                {
                    buffAnimalSelect = buffAnimalSelect + "AND BLOOD_CODE = @BloodCode ";
                }

                if (searchFilterModel.filterBy.BreedCode != null && searchFilterModel.filterBy.BreedCode != "")
                {
                    buffAnimalSelect = buffAnimalSelect + "AND BREED_CODE = @BreedCode ";
                }

                if (searchFilterModel.filterBy.TypeOfOwnership != null && searchFilterModel.filterBy.TypeOfOwnership != "")
                {
                    buffAnimalSelect = buffAnimalSelect + "AND TYPE_OF_OWNERSHIP = @TypeOfOwnership ";
                }
            }

            return buffAnimalSelect;
        }

        public static String buildBuffAnimalSearchAll()
        {
            String buffAnimalSelect = Constants.DBQuery.BUFF_ANIMAL_SELECT + "WHERE DELETE_FLAG = 0 ";
            return buffAnimalSelect;
        }

        public static String buildBuffAnimalSearchByReferenceNumber(String referencenUmber)
        {
            String buffAnimalSelect = Constants.DBQuery.BUFF_ANIMAL_SELECT + "INNER JOIN TBL_DAMMODEL AS DM ON BA.Dam_ID = DM.ID WHERE DELETE_FLAG = 0 AND ( DM.DAM_REGISTRATION_NUMBER = '" + referencenUmber + "' OR BA.RFID_NUMBER = '" + referencenUmber + "')";
            return buffAnimalSelect;
        }

        public static String buildBuffAnimalDuplicateQuery(BuffAnimalRegistrationModel buffAnimalRegistrationModel)
        {
            String buffAnimalDuplicateQuery = Constants.DBQuery.BUFF_ANIMAL_SELECT + "WHERE DELETE_FLAG = 0 AND ( HERD_CODE = '" + buffAnimalRegistrationModel.HerdCode + "' AND ANIMAL_ID_Number = '" + buffAnimalRegistrationModel.AnimalIdNumber + "')";
            return buffAnimalDuplicateQuery;
        }

        public static String buildBuffAnimalSearchById(int id)
        {
            String buffAnimalDuplicateQuery = Constants.DBQuery.BUFF_ANIMAL_SELECT + "WHERE DELETE_FLAG = 0 AND id = " + id;
            return buffAnimalDuplicateQuery;
        }

        public static String buildBuffAnimalSelectDuplicateQueryByIdAnimalIdNumberName(int id, String animalIdNumber, String animalName)
        {
            return Constants.DBQuery.BUFF_ANIMAL_SELECT + "WHERE DELETE_FLAG = 0 AND NOT id = " + id + "  AND ANIMAL_ID_NUMBER = '" + animalIdNumber + "' AND ANIMAL_NAME = '" + animalName + "'";
        }

        public static String buildSireSearchQueryById(int id)
        {
            return Constants.DBQuery.SIRE_TABLE_SELECT + "WHERE id = " + id;
        }

        public static String buildSireSearchQueryBySire(BuffAnimalRegistrationModel buffAnimalRegistrationModel)
        {
            return Constants.DBQuery.SIRE_TABLE_SELECT + "WHERE " +
                "SIRE_REGISTRATION_NUMBER = '" + buffAnimalRegistrationModel.Sire.RegistrationNumber + "' " +
                "AND SIRE_ID_NUMBER = '" + buffAnimalRegistrationModel.Sire.IdNumber + "' " +
                "AND SIRE_NAME = '" + buffAnimalRegistrationModel.Sire.Name + "' " +
                "AND BREED_CODE = '" + buffAnimalRegistrationModel.Sire.BreedCode + "' " +
                "AND BLOOD_CODE = '" + buffAnimalRegistrationModel.Sire.BloodCode + "'";
        }

        public static String buildDamSearchQueryById(int id)
        {
            return Constants.DBQuery.DAM_TABLE_SELECT + "WHERE id = " + id;
        }

        public static String buildDamSearchQueryByRegNumIdNumName(BuffAnimalRegistrationModel buffAnimalRegistrationModel)
        {
            return Constants.DBQuery.DAM_TABLE_SELECT + "WHERE " +
                "DAM_REGISTRATION_NUMBER = '" + buffAnimalRegistrationModel.Dam.RegistrationNumber + "' " +
                "AND DAM_ID_NUMBER = '" + buffAnimalRegistrationModel.Dam.IdNumber + "' " +
                "AND DAM_NAME = '" + buffAnimalRegistrationModel.Dam.Name + "'" +
                "AND BREED_CODE = '" + buffAnimalRegistrationModel.Dam.BreedCode + "' " +
                "AND BLOOD_CODE = '" + buffAnimalRegistrationModel.Dam.BloodCode + "'";
        }

        public static String buildOriginAcquisitionSearchQueryById(int id)
        {
            return Constants.DBQuery.ORIGIN_ACQUISITION_SELECT + "WHERE ID = " + id;
        }

        public static String buildOriginAcquisitionSearchQueryByOriginAcquistion(BuffAnimalRegistrationModel buffAnimalRegistrationModel)
        {
            return Constants.DBQuery.ORIGIN_ACQUISITION_SELECT + "WHERE " +
                            "OA.CITY = '" + buffAnimalRegistrationModel.OriginOfAcquisition.City + "' " +
                            "AND OA.PROVINCE = '" + buffAnimalRegistrationModel.OriginOfAcquisition.Province+ "' " +
                            "AND OA.BARANGAY = '" + buffAnimalRegistrationModel.OriginOfAcquisition.Barangay + "' " +
                            "AND OA.REGION = '" + buffAnimalRegistrationModel.OriginOfAcquisition.Region + "'" ;
        }

        public static String buildFarmerAffiliationSearchQuery(CommonSearchFilterModel searchFilterModel)
        {
            String herdSelect = Constants.DBQuery.FARMER_AFFILIATION_SELECT + "WHERE DELETE_FLAG = 0 ";
            if (searchFilterModel.searchParam != null && searchFilterModel.searchParam != "")
            {
                herdSelect = herdSelect + "AND (F_Code LIKE '%' + @SearchParam + '%' OR F_DESC LIKE '%' + @SearchParam +'%') ";
            }
            return herdSelect;
        }

        public static String buildFarmerAffiliationSearchQueryAll()
        {
            String farmerAffiliationSearchQueryByFCode = Constants.DBQuery.FARMER_AFFILIATION_SELECT + "WHERE DELETE_FLAG = 0";

            return farmerAffiliationSearchQueryByFCode;
        }

        public static String buildFarmerAffiliationDeletedSearchQueryById()
        {
            String farmerAffiliationSearchQueryByFCode = Constants.DBQuery.FARMER_AFFILIATION_SELECT + "WHERE DELETE_FLAG = 1 AND ID = @Id ";

            return farmerAffiliationSearchQueryByFCode;
        }

        public static String buildFarmerAffiliationSearchQueryById()
        {
            String farmerAffiliationSearchQueryByFCode = Constants.DBQuery.FARMER_AFFILIATION_SELECT + "WHERE DELETE_FLAG = 0 AND ID = @Id ";

            return farmerAffiliationSearchQueryByFCode;
        }


        public static String buildFarmerAffiliationSearchQueryByFCode()
        {
            String farmerAffiliationSearchQueryByFCode = Constants.DBQuery.FARMER_AFFILIATION_SELECT + "WHERE DELETE_FLAG = 0 AND F_CODE = @FCode ";
            
            return farmerAffiliationSearchQueryByFCode;
        }

        public static String buildFarmerAffiliationDuplicateCheckSaveQuery()
        {
            String farmerAffiliationDuplicateCheck = Constants.DBQuery.FARMER_AFFILIATION_SELECT + "WHERE DELETE_FLAG = 0 AND F_CODE = @FCode AND F_DESC = @FDesc";
            return farmerAffiliationDuplicateCheck;
        }

        public static String buildFarmerAffiliationDuplicateCheckUpdateQuery()
        {
            String farmerAffiliationDuplicateCheck = Constants.DBQuery.FARMER_AFFILIATION_SELECT + "WHERE DELETE_FLAG = 0 AND ID <> @Id AND F_CODE = @FCode AND F_DESC = @FDesc";
            return farmerAffiliationDuplicateCheck;
        }

        public static String buildHerdClassificationSearchQuery(CommonSearchFilterModel searchFilterModel)
        {
            String herdSelect = Constants.DBQuery.HERD_CLASSIFICATION_SELECT + "WHERE DELETE_FLAG = 0 ";
            if (searchFilterModel.searchParam != null && searchFilterModel.searchParam != "")
            {
                herdSelect = herdSelect + "AND (Herd_Class_Code LIKE '%' + @SearchParam + '%' OR Herd_Class_Desc LIKE '%' + @SearchParam +'%') ";
            }
            return herdSelect;
        }

        public static String buildHerdClassificationSearchQueryAll()
        {
            String herdClassificationSelect = Constants.DBQuery.HERD_CLASSIFICATION_SELECT + "WHERE DELETE_FLAG = 0";
            return herdClassificationSelect;
        }

        public static String buildHerdClassificationDeletedSearchQueryById()
        {
            String herdClassificationSelect = Constants.DBQuery.HERD_CLASSIFICATION_SELECT + "WHERE DELETE_FLAG = 1 AND ID = @Id";
            return herdClassificationSelect;
        }

        public static String buildHerdClassificationSearchQueryById()
        {
            String herdClassificationSelect = Constants.DBQuery.HERD_CLASSIFICATION_SELECT + "WHERE DELETE_FLAG = 0 AND ID = @Id";
            return herdClassificationSelect;
        }

        public static String buildHerdClassificationSearchQueryByHerdClassCode()
        {
            String herdClassificationSelect = Constants.DBQuery.HERD_CLASSIFICATION_SELECT + "WHERE DELETE_FLAG = 0 AND HERD_CLASS_CODE = @HerdClassCode";
            return herdClassificationSelect;
        }

        public static String buildHerdClassificationSearchQueryByHerdClassDesc()
        {
            String herdClassificationSelect = Constants.DBQuery.HERD_CLASSIFICATION_SELECT + "WHERE DELETE_FLAG = 0 AND HERD_CLASS_DESC = @HerdClassDesc";
            return herdClassificationSelect;
        }

        public static String buildHerdClassificationDuplicateCheckSaveQuery()
        {
            String herdClassificationSelect = Constants.DBQuery.HERD_CLASSIFICATION_SELECT + "WHERE DELETE_FLAG = 0 AND HERD_CLASS_CODE = @HerdClassCode AND HERD_CLASS_DESC = @HerdClassDesc";
            return herdClassificationSelect;
        }

        public static String buildHerdClassificationDuplicateCheckUpdateQuery()
        {
            String herdClassificationSelect = Constants.DBQuery.HERD_CLASSIFICATION_SELECT + "WHERE DELETE_FLAG = 0 AND ID <> @Id AND HERD_CLASS_CODE = @HerdClassCode AND HERD_CLASS_DESC = @HerdClassDesc";
            return herdClassificationSelect;
        }


        // Breed Query
        public static String buildBreedSearchQuery(CommonSearchFilterModel searchFilterModel)
        {
            String breedSearchQuery = Constants.DBQuery.BREED_SELECT + "WHERE DELETE_FLAG = 0 ";
            if (searchFilterModel.searchParam != null && searchFilterModel.searchParam != "")
            {
                breedSearchQuery = breedSearchQuery + "AND (Breed_Code LIKE '%' + @SearchParam + '%' OR Breed_Desc LIKE '%' + @SearchParam +'%') ";
            }
            return breedSearchQuery;
        }

        public static String buildBreedSearchQueryAll()
        {
            return Constants.DBQuery.BREED_SELECT + "WHERE DELETE_FLAG = 0";
        }

        public static String buildBreedDeletedSearchQueryById()
        {
            return Constants.DBQuery.BREED_SELECT + "WHERE DELETE_FLAG = 1 AND ID = @Id";
        }

        public static String buildBreedSearchQueryById()
        {
            return Constants.DBQuery.BREED_SELECT + "WHERE DELETE_FLAG = 0 AND ID = @Id";
        }

        public static String buildBreedSearchQueryByBreedCode()
        {
            return Constants.DBQuery.BREED_SELECT + "WHERE DELETE_FLAG = 0 AND BREED_CODE = @BreedCode";
        }

        public static String buildBreedDuplicateCheckSaveQuery()
        {
            return Constants.DBQuery.BREED_SELECT + "WHERE DELETE_FLAG = 0 AND Breed_Code = @BreedCode AND Breed_Desc = @BreedDesc ";
        }

        public static String buildBreedDuplicateCheckUpdateQuery()
        {
            return Constants.DBQuery.BREED_SELECT + "WHERE DELETE_FLAG = 0 AND ID <> @Id AND Breed_Code = @BreedCode AND Breed_Desc = @BreedDesc ";
        }

        public static String buildFeedingSystemSearchQuery(CommonSearchFilterModel searchFilterModel)
        {
            String herdSelect = Constants.DBQuery.FEEDING_SYSTEM_SELECT + "WHERE DELETE_FLAG = 0 ";
            if (searchFilterModel.searchParam != null && searchFilterModel.searchParam != "")
            {
                herdSelect = herdSelect + "AND (Feeding_System_Code LIKE '%' + @SearchParam + '%' OR Feeding_System_Desc LIKE '%' + @SearchParam +'%') ";
            }
            return herdSelect;
        }
        public static String buildFeedingSystemSearchByFeedingSystemCode()
        {
            return Constants.DBQuery.FEEDING_SYSTEM_SELECT + "WHERE DELETE_FLAG = 0  AND FEED_CODE = @FeedCode";
        }

        public static String buildBuffaloTypeSearchQuery(CommonSearchFilterModel searchFilterModel)
        {
            String herdSelect = Constants.DBQuery.BUFFALO_TYPE_SELECT + "WHERE DELETE_FLAG = 0 ";
            if (searchFilterModel.searchParam != null && searchFilterModel.searchParam != "")
            {
                herdSelect = herdSelect + "AND (Breed_Type_Code LIKE '%' + @SearchParam + '%' OR Breed_Type_Desc LIKE '%' + @SearchParam +'%') ";
            }
            return herdSelect;
        }

        public static String buildBuffaloTypeSearchQueryAll()
        {
            return Constants.DBQuery.BUFFALO_TYPE_SELECT + "WHERE DELETE_FLAG = 0";
        }

        public static String buildBuffaloTypeDeletedSearchQueryById()
        {
            return Constants.DBQuery.BUFFALO_TYPE_SELECT + "WHERE DELETE_FLAG = 1 AND ID = @Id";
        }

        public static String buildBuffaloTypeSearchQueryById()
        {
            return Constants.DBQuery.BUFFALO_TYPE_SELECT + "WHERE DELETE_FLAG = 0 AND ID = @Id";
        }

        public static String buildBuffaloTypeSearchQueryByBreedTypeCode()
        {
            return Constants.DBQuery.BUFFALO_TYPE_SELECT + "WHERE DELETE_FLAG = 0 AND BREED_TYPE_CODE = @BreedTypeCode";
        }

        public static String buildBuffaloTypeDuplicateCheckSaveQuery()
        {
            return Constants.DBQuery.BUFFALO_TYPE_SELECT + "WHERE DELETE_FLAG = 0 AND BREED_TYPE_CODE = @BreedTypeCode AND BREED_TYPE_DESC = @BreedTypeDesc ";
        }

        public static String buildBuffaloTypeDuplicateCheckUpdateQuery()
        {
            return Constants.DBQuery.BUFFALO_TYPE_SELECT + "WHERE DELETE_FLAG = 0 AND ID <> @Id AND BREED_TYPE_CODE = @BreedTypeCode AND BREED_TYPE_DESC = @BreedTypeDesc ";
        }

        public static String buildUserSearchQuery(CommonSearchFilterModel searchFilterModel)
        {
            String userSelect = Constants.DBQuery.USERS_SELECT + "WHERE DELETE_FLAG = 0 ";
            if (searchFilterModel.searchParam != null && searchFilterModel.searchParam != "")
            {
                userSelect = userSelect + "AND (Fname LIKE '%' + @SearchParam + '%' OR Lname LIKE '%' + @SearchParam +'%' OR Mname LIKE '%' + @SearchParam +'%' OR Email LIKE '%' + @SearchParam +'%') ";
            }
            return userSelect;
        }
        public static String buildUserSearchQueryById()
        {
            return Constants.DBQuery.USERS_SELECT + "WHERE DELETE_FLAG = 0  AND Id = @Id";
        }

        public static String buildUserSearchQuery()
        {
            return Constants.DBQuery.USERS_SELECT + "WHERE DELETE_FLAG = 0  AND USERNAME = @Username";
        }

        public static String buildUserDeletedSearchQueryById()
        {
            return Constants.DBQuery.USERS_SELECT + "WHERE DELETE_FLAG = 1 AND ID = @Id";
        }

        public static String buildUserForApprovalSearchQuery(CommonSearchFilterModel searchFilterModel)
        {
            String userSelect = Constants.DBQuery.USERS_SELECT + "WHERE DELETE_FLAG = 0 AND Status = 3";
            if (searchFilterModel.searchParam != null && searchFilterModel.searchParam != "")
            {
                userSelect = userSelect + "AND (Fname LIKE '%' + @SearchParam + '%' OR Lname LIKE '%' + @SearchParam +'%' OR Mname LIKE '%' + @SearchParam +'%' OR Email LIKE '%' + @SearchParam +'%') ";
            }
            return userSelect;
        }

        public static String buildUserDuplicateCheckUpdateQuery()
        {
            return Constants.DBQuery.USERS_SELECT + "WHERE DELETE_FLAG = 0 AND ID <> @Id AND (USERNAME = @Username OR (Fullname = @Fullname AND Fname = @Fname AND Lname = @Lname AND Mname = @Mname AND Email = @Email))";
        }

        public static String buildBirthTypeSearchQueryByBirthTypeCodeOrBirthTypeDesc()
        {
            return Constants.DBQuery.BIRTH_TYPE_SELECT + "WHERE DELETE_FLAG = 0 AND (BIRTH_TYPE_CODE = @SearchParam AND BIRTH_TYPE_DESC = @SearchParam)";
        }

        public static String buildUserTypeSearchQuery(CommonSearchFilterModel searchFilterModel)
        {
            String userTypeSelect = Constants.DBQuery.USER_TYPE_TABLE_SELECT + "WHERE DELETE_FLAG = 0 ";
            if (searchFilterModel.searchParam != null && searchFilterModel.searchParam != "")
            {
                userTypeSelect = userTypeSelect + "AND (CODE = @SearchParam OR NAME = @SearchParam) ";
            }
            return userTypeSelect;
        }

        public static String buildUserTypeQueryByCodeOrName()
        {
            return Constants.DBQuery.USER_TYPE_TABLE_SELECT + "WHERE DELETE_FLAG = 0 AND (CODE = @SearchParam OR NAME = @SearchParam)";
        }

        public static String buildUserTypeQueryByName()
        {
            return Constants.DBQuery.USER_TYPE_TABLE_SELECT + "WHERE DELETE_FLAG = 0 AND NAME = @Name";
        }

        public static String buildUserTypeQueryById()
        {
            return Constants.DBQuery.USER_TYPE_TABLE_SELECT + "WHERE DELETE_FLAG = 0 AND ID = @Id";
        }

        public static String buildUserTypeDuplicateCheckUpdateQuery()
        {
            return Constants.DBQuery.USER_TYPE_TABLE_SELECT + "WHERE DELETE_FLAG = 0 AND ID <> @Id AND (Code = @Code AND Name = @Name)";
        }

        public static String buildUserTypeDuplicateCheckSaveQuery()
        {
            return Constants.DBQuery.USER_TYPE_TABLE_SELECT + "WHERE DELETE_FLAG = 0 AND (Code = @Code AND Name = @Name)";
        }

        public static String buildUserTypeDeletedSearchQueryById()
        {
            return Constants.DBQuery.USER_TYPE_TABLE_SELECT + "WHERE DELETE_FLAG = 1 AND ID = @Id";
        }
    }

}
