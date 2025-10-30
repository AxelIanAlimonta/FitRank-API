namespace FitRank_API.Application.DTOs.RutinaDTOs
{
    public sealed class ScreeningDTO
    {
        public bool Hipertension { get; init; }
        public bool HipertensionControlada { get; init; }
        public bool Diabetes { get; init; }
        public bool CirugiaReciente { get; init; }
        public bool DolorLumbar { get; init; }
        public bool DolorHombro { get; init; }
        public bool DolorRodilla { get; init; }
        public bool DolorToracico { get; init; }
        public bool Sincope { get; init; }
        public bool Embarazo { get; init; }

        public int FrecuenciaCardiacaReposo { get; init; }
        public int DolorEscala0a10 { get; init; }
    }
}
