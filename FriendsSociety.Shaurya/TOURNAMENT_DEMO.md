# Tournament Entity Implementation

This document demonstrates the Tournament entity implementation that addresses Issue #9.

## Overview

The Tournament entity serves as a parent entity that can contain multiple Activities (games). This creates a hierarchical structure:

```
Tournament
├── Activity 1 (Game 1)
│   └── GroundAllocations (with Ground assignments)
├── Activity 2 (Game 2)
│   └── GroundAllocations (with Ground assignments)
└── Activity 3 (Game 3)
    └── GroundAllocations (with Ground assignments)
```

## Entity Structure

### Tournament Entity
- **TournamentID**: Primary key
- **Name**: Tournament name (required)
- **Description**: Optional description
- **StartDate**: Tournament start date
- **EndDate**: Tournament end date
- **Location**: Optional tournament location
- **IsActive**: Whether tournament is currently active
- **IsDeleted**: Soft delete flag
- **CreatedAt**: Creation timestamp
- **UpdatedAt**: Last update timestamp
- **Activities**: Navigation property to related activities

### Activity Entity Updates
- Added **TournamentID**: Foreign key to Tournament (nullable)
- Added **Tournament**: Navigation property to parent tournament

## API Endpoints

### Tournament Management
- `GET /api/tournaments` - Get all active tournaments
- `GET /api/tournaments/{id}` - Get specific tournament with activities
- `POST /api/tournaments` - Create new tournament
- `PUT /api/tournaments/{id}` - Update tournament
- `DELETE /api/tournaments/{id}` - Soft delete tournament

### Tournament-Activity Management
- `GET /api/tournaments/{id}/activities` - Get all activities for a tournament
- `POST /api/tournaments/{tournamentId}/activities/{activityId}` - Add activity to tournament
- `DELETE /api/tournaments/{tournamentId}/activities/{activityId}` - Remove activity from tournament

## Usage Examples

### Creating a Tournament
```json
POST /api/tournaments
{
  "name": "Summer Sports Tournament 2025",
  "description": "Annual summer tournament featuring multiple sports",
  "startDate": "2025-07-01T09:00:00Z",
  "endDate": "2025-07-15T18:00:00Z",
  "location": "Central Sports Complex",
  "isActive": true
}
```

### Adding Activities to Tournament
```json
POST /api/tournaments/1/activities/5
```

### Getting Tournament with Activities
```json
GET /api/tournaments/1
```

Response includes:
- Tournament details
- All associated activities
- Ground allocations for each activity
- Ground details for each allocation

## Database Relationships

The implementation includes proper Entity Framework relationships:
- **One-to-Many**: Tournament → Activities
- **Foreign Key**: Activities.TournamentID references Tournaments.TournamentID
- **Delete Behavior**: SetNull (when tournament is deleted, activities remain but TournamentID is set to null)

## Benefits

1. **Hierarchical Organization**: Activities can now be grouped under tournaments
2. **Flexible Design**: Activities can exist independently or as part of a tournament
3. **Data Integrity**: Proper foreign key relationships with appropriate delete behavior
4. **Complete CRUD**: Full API operations for tournament management
5. **Soft Deletes**: Non-destructive deletion preserves data integrity
6. **Audit Trail**: Created and updated timestamps for tracking