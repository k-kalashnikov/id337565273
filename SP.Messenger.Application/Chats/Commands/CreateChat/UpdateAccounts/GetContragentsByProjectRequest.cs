using System;

namespace SP.Consumers.Models
{
    public class GetContragentsByProjectRequest
    {
        public GetContragentsByProjectRequest(Guid projectId)
        {
            ProjectId = projectId;
        }
        public Guid ProjectId { get; }

        public static GetContragentsByProjectRequest Create(Guid projectId)
            => new GetContragentsByProjectRequest(projectId);
    }
}