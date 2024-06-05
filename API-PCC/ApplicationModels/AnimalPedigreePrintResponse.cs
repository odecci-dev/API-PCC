namespace API_PCC.ApplicationModels
{
    public class AnimalPedigreePrintResponse
    {
        public AnimalDetails animalDetails { get; set; }
        public AnimalPedigreeTree<AnimalPedigreeModel> animalPedigree { get; set; }
        public HerdDetails herdDetails { get; set; }

    }
}
