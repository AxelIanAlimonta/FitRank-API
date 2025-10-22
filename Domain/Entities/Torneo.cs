namespace FitRank_API.Domain.Entities
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Text.Json;
    using System.Text.Json.Serialization;

    namespace TuProyecto.Models
    {
        public class Torneo
        {
            [Key]
            public int Id { get; set; }
            public string Nombre { get; set; }
            [JsonIgnore]
            public string ParticipantesJson { get; set; } = "[]";

            // Propiedad auxiliar para trabajar con el JSON como lista
            [NotMapped]
            [JsonIgnore]
            public List<Participante> Participantes
            {
                get => string.IsNullOrEmpty(ParticipantesJson)
                    ? new List<Participante>()
                    : JsonSerializer.Deserialize<List<Participante>>(ParticipantesJson);
                set => ParticipantesJson = JsonSerializer.Serialize(value);
            }
        }

        // Clase interna para los datos del participante
        [NotMapped]
        public class Participante
        {
            public string Nombre { get; set; }
            public int Puntaje { get; set; }
        }
    }
}
