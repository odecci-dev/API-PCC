namespace API_PCC.Constants
{
    public static class DBQuery
    {
        public static readonly String BIRTH_TYPE_SELECT = $@"SELECT * FROM A_BIRTH_TYPE ";
        public static readonly String BLOOD_COMPOSITION_SELECT = @"SELECT * FROM A_BLOOD_COMP ";
        public static readonly String BREED_SELECT = $@"SELECT * FROM A_BREED ";
        public static readonly String BUFF_ANIMAL_SELECT = $@"SELECT BA.* FROM A_BUFF_ANIMAL AS BA ";
        public static readonly String TYPE_OWNERSHIP_SELECT = $@"SELECT * FROM A_TYPE_OWNERSHIP ";
        public static readonly String HERD_SELECT = $@"SELECT * FROM H_BUFF_HERD ";
        /*public static readonly String HERD_SELECT = $@"SELECT  H_Buff_Herd.id,       H_Buff_Herd.Herd_Name, H_Herd_Classification.Herd_Class_Desc, H_Herd_Classification.Herd_Class_Code, H_Buff_Herd.Herd_Size, H_Buff_Herd.Farm_Manager, H_Buff_Herd.Herd_Code, H_Buff_Herd.Date_Created, H_Buff_Herd.Breed_Type_Code, 
                         H_Buff_Herd.Farm_Affil_Code, H_Buff_Herd.Feeding_System_Code, H_Buff_Herd.Farm_Address, H_Buff_Herd.Owner, H_Buff_Herd.Status, H_Buff_Herd.Date_Updated, H_Buff_Herd.Created_By, H_Buff_Herd.Delete_Flag, 
                         H_Buff_Herd.Updated_By, H_Buff_Herd.Date_Deleted, H_Buff_Herd.Date_Restored, H_Buff_Herd.Deleted_By, H_Buff_Herd.Restored_By, H_Buff_Herd.Center, H_Buff_Herd.Organization_name, H_Buff_Herd.Photo
FROM            H_Buff_Herd INNER JOIN
                         H_Herd_Classification ON H_Buff_Herd.Herd_Class_Desc = H_Herd_Classification.HERD_CLASS_DESC ";*/
        public static readonly String BUFFALO_TYPE_SELECT = $@"SELECT * FROM H_BUFFALO_TYPE ";
        public static readonly String FARMER_AFFILIATION_SELECT = $@"SELECT * FROM H_FARMER_AFFILIATION ";
        public static readonly String FEEDING_SYSTEM_SELECT = $@"SELECT * FROM H_FEEDING_SYSTEM ";
        public static readonly String HERD_CLASSIFICATION_SELECT = $@"SELECT * FROM H_HERD_ClASSIFICATION ";

        public static readonly String CENTER_SELECT = $@"SELECT * FROM TBL_CENTERMODEL ";
        public static readonly String FARM_OWNER_SELECT = $@"SELECT * FROM TBL_FARMOWNER ";
        public static readonly String USERS_SELECT = $@"SELECT * FROM TBL_USERSMODEL ";
        public static readonly String MAIL_SENDER_CREDENTIALS_SELECT = $@"SELECT * FROM TBL_MAILSENDERCREDENTIAL ";
        public static readonly String REGISTRATION_OTP_SELECT = $@"SELECT * FROM TBL_REGISTRATIONOTPMODEL ";

        public static readonly String ACTION_TABLE_SELECT = $@"SELECT * FROM ACTION_TBL ";
        public static readonly String MODULE_TABLE_SELECT = $@"SELECT * FROM MODULE_TBL ";

        public static readonly String SIRE_TABLE_SELECT = $@"SELECT * FROM TBL_SIREMODEL ";
        public static readonly String DAM_TABLE_SELECT = $@"SELECT * FROM TBL_DAMMODEL ";

        public static readonly String ORIGIN_ACQUISITION_SELECT = $@"SELECT OA.* FROM TBL_ORIGINOFACQUISITIONMODEL OA ";

        public static readonly String USER_TYPE_TABLE_SELECT = $@"SELECT * FROM TBL_USERTYPEMODEL ";

    }
}
