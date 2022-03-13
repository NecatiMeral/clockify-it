﻿using System.Collections.Generic;

namespace Sg.ClockifyIt.Integrations.Dtos
{
    public class TaskDto
    {
        public List<string> AssigneeIds { get; set; }

        public string Estimate { get; set; }

        public string Id { get; set; }

        public string Name { get; set; }

        public string ProjectId { get; set; }

        public TaskStatus Status { get; set; }
    }
}
