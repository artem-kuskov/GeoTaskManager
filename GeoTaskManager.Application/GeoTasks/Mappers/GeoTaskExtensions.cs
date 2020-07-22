using GeoTaskManager.Application.Actors.Mappers;
using GeoTaskManager.Application.Actors.Models;
using GeoTaskManager.Application.Core.Responses;
using GeoTaskManager.Application.GeoTasks.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using _TJsonPatchOperation = Microsoft.AspNetCore.JsonPatch.Operations
                             .OperationType;

namespace GeoTaskManager.Application.GeoTasks.Mappers
{
    public static class GeoTaskExtensions
    {
        public static EntityResponse<GeoTask> ToGeoTaskResponse
            (this GeoTask from)
        {
            if (from is null)
            {
                return null;
            }

            var response = new EntityResponse<GeoTask>
            {
                Entity = from,
                Success = true
            };

            return response;
        }

        public static Dictionary<string, object> ToDictionary
            (this GeoTask from)
        {
            if (from is null)
            {
                return null;
            }

            return new Dictionary<string, object>
            {
                { nameof(from.AssistentActors),
                    String.Join(',', from.AssistentActors.Select(x => x.Id)) },
                { nameof(from.CreatedAt), from.CreatedAt },
                { nameof(from.CreatedBy),
                    String.Join(',', from.CreatedBy
                        .ToDictionary()
                        .Select(x => $"{x.Key}={x.Value}"))},
                { nameof(from.Description), from.Description },
                { nameof(from.GeosIds),
                    String.Join(',', from.GeosIds) },
                { nameof(from.History),
                    String.Join(',', from.History.Select(x => x.Id)) },
                { nameof(from.Id),  from.Id },
                { nameof(from.IsArchived),  from.IsArchived },
                { nameof(from.ObserverActors),
                    String.Join(',', from.ObserverActors.Select(x => x.Id)) },
                { nameof(from.PlanFinishAt), from.PlanFinishAt },
                { nameof(from.PlanStartAt), from.PlanStartAt },
                { nameof(from.ProjectId), from.ProjectId },
                { nameof(from.ResponsibleActor), from.ResponsibleActor?.Id },
                { nameof(from.Status), from.Status?.Id },
                { nameof(from.StatusChangedAt), from.StatusChangedAt },
                { nameof(from.Title), from.Title },
            };
        }

        public static IEnumerable<Operation> ToHistoryOperations
            (this GeoTask newGeoTask, GeoTask oldGeoTask)
        {
            if (newGeoTask?.CreatedAt != oldGeoTask?.CreatedAt)
            {
                yield return new Operation
                {
                    NewValue = newGeoTask?.CreatedAt,
                    OldValue = oldGeoTask?.CreatedAt,
                    OperationType = _TJsonPatchOperation.Replace,
                    Path = $"/{nameof(newGeoTask.CreatedAt)}"
                };
            };

            if (newGeoTask?.CreatedBy != oldGeoTask?.CreatedBy)
            {
                yield return new Operation
                {
                    NewValue = newGeoTask?.CreatedBy,
                    OldValue = oldGeoTask?.CreatedBy,
                    OperationType = _TJsonPatchOperation.Replace,
                    Path = $"/{nameof(newGeoTask.CreatedBy)}"
                };
            };

            if (newGeoTask?.Description != oldGeoTask?.Description)
            {
                yield return new Operation
                {
                    NewValue = newGeoTask?.Description,
                    OldValue = oldGeoTask?.Description,
                    OperationType = _TJsonPatchOperation.Replace,
                    Path = $"/{nameof(newGeoTask.Description)}"
                };
            };

            if (newGeoTask?.Id != oldGeoTask?.Id)
            {
                yield return new Operation
                {
                    NewValue = newGeoTask?.Id,
                    OldValue = oldGeoTask?.Id,
                    OperationType = _TJsonPatchOperation.Replace,
                    Path = $"/{nameof(newGeoTask.Id)}"
                };
            };

            if (newGeoTask?.IsArchived != oldGeoTask?.IsArchived)
            {
                yield return new Operation
                {
                    NewValue = newGeoTask?.IsArchived,
                    OldValue = oldGeoTask?.IsArchived,
                    OperationType = _TJsonPatchOperation.Replace,
                    Path = $"/{nameof(newGeoTask.IsArchived)}"
                };
            };

            if (newGeoTask?.PlanFinishAt != oldGeoTask?.PlanFinishAt)
            {
                yield return new Operation
                {
                    NewValue = newGeoTask?.PlanFinishAt,
                    OldValue = oldGeoTask?.PlanFinishAt,
                    OperationType = _TJsonPatchOperation.Replace,
                    Path = $"/{nameof(newGeoTask.PlanFinishAt)}"
                };
            };

            if (newGeoTask?.PlanStartAt != oldGeoTask?.PlanStartAt)
            {
                yield return new Operation
                {
                    NewValue = newGeoTask?.PlanStartAt,
                    OldValue = oldGeoTask?.PlanStartAt,
                    OperationType = _TJsonPatchOperation.Replace,
                    Path = $"/{nameof(newGeoTask.PlanStartAt)}"
                };
            };

            if (newGeoTask?.ProjectId != oldGeoTask?.ProjectId)
            {
                yield return new Operation
                {
                    NewValue = newGeoTask?.ProjectId,
                    OldValue = oldGeoTask?.ProjectId,
                    OperationType = _TJsonPatchOperation.Replace,
                    Path = $"/{nameof(newGeoTask.ProjectId)}"
                };
            };

            if (newGeoTask?.ResponsibleActor != oldGeoTask?.ResponsibleActor)
            {
                yield return new Operation
                {
                    NewValue = newGeoTask?.ResponsibleActor,
                    OldValue = oldGeoTask?.ResponsibleActor,
                    OperationType = _TJsonPatchOperation.Replace,
                    Path = $"/{nameof(newGeoTask.ResponsibleActor)}"
                };
            };

            if (newGeoTask?.Status != oldGeoTask?.Status)
            {
                yield return new Operation
                {
                    NewValue = newGeoTask?.Status,
                    OldValue = oldGeoTask?.Status,
                    OperationType = _TJsonPatchOperation.Replace,
                    Path = $"/{nameof(newGeoTask.Status)}"
                };
            };

            if (newGeoTask?.StatusChangedAt != oldGeoTask?.StatusChangedAt)
            {
                yield return new Operation
                {
                    NewValue = newGeoTask?.StatusChangedAt,
                    OldValue = oldGeoTask?.StatusChangedAt,
                    OperationType = _TJsonPatchOperation.Replace,
                    Path = $"/{nameof(newGeoTask.StatusChangedAt)}"
                };
            };

            if (newGeoTask?.Title != oldGeoTask?.Title)
            {
                yield return new Operation
                {
                    NewValue = newGeoTask?.Title,
                    OldValue = oldGeoTask?.Title,
                    OperationType = _TJsonPatchOperation.Replace,
                    Path = $"/{nameof(newGeoTask.Title)}"
                };
            };

            var newAssistenceActorsSet = newGeoTask?.AssistentActors.ToHashSet()
                ?? new HashSet<Actor>();
            var oldAssistenceActorsSet = oldGeoTask?.AssistentActors.ToHashSet()
                ?? new HashSet<Actor>();
            var addedAssistenceActors = newAssistenceActorsSet
                .Except(oldAssistenceActorsSet);
            var removedAssistenceActors = oldAssistenceActorsSet
                .Except(newAssistenceActorsSet);
            foreach (var item in removedAssistenceActors)
            {
                yield return new Operation
                {
                    OldValue = item,
                    OperationType = _TJsonPatchOperation.Remove,
                    Path = $"/{nameof(newGeoTask.AssistentActors)}"
                };
            }
            foreach (var item in addedAssistenceActors)
            {
                yield return new Operation
                {
                    NewValue = item,
                    OperationType = _TJsonPatchOperation.Add,
                    Path = $"/{nameof(newGeoTask.AssistentActors)}"
                };
            }

            var newObserverActorsSet = newGeoTask?.ObserverActors.ToHashSet()
                ?? new HashSet<Actor>();
            var oldObserverActorsSet = oldGeoTask?.ObserverActors.ToHashSet()
                ?? new HashSet<Actor>();
            var addedObserverActors = newObserverActorsSet
                .Except(oldObserverActorsSet);
            var removedObserverActors = oldObserverActorsSet
                .Except(newObserverActorsSet);
            foreach (var item in removedObserverActors)
            {
                yield return new Operation
                {
                    OldValue = item,
                    OperationType = _TJsonPatchOperation.Remove,
                    Path = $"/{nameof(newGeoTask.ObserverActors)}"
                };
            }
            foreach (var item in addedObserverActors)
            {
                yield return new Operation
                {
                    NewValue = item,
                    OperationType = _TJsonPatchOperation.Add,
                    Path = $"/{nameof(newGeoTask.ObserverActors)}"
                };
            }

            var newGeosIdsSet = newGeoTask?.GeosIds.ToHashSet()
                ?? new HashSet<string>();
            var oldGeosIdsSet = oldGeoTask?.GeosIds.ToHashSet()
                ?? new HashSet<string>();
            var addedGeosIds = newGeosIdsSet.Except(oldGeosIdsSet);
            var removedGeosIds = oldGeosIdsSet.Except(newGeosIdsSet);
            foreach (var item in removedGeosIds)
            {
                yield return new Operation
                {
                    OldValue = item,
                    OperationType = _TJsonPatchOperation.Remove,
                    Path = $"/{nameof(newGeoTask.GeosIds)}"
                };
            }
            foreach (var item in addedGeosIds)
            {
                yield return new Operation
                {
                    NewValue = item,
                    OperationType = _TJsonPatchOperation.Add,
                    Path = $"/{nameof(newGeoTask.GeosIds)}"
                };
            }
        }
    }
}
