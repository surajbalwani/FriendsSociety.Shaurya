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

## Seed Data for Testing

The implementation includes comprehensive seed data in `ModelSeeder.cs`:

### Sample Tournaments
1. **Spring Adaptive Sports Championship 2025**
   - Multi-day championship (May 1-3, 2025)
   - Features Wheelchair Basketball and Blind Running
   - Located at Central Sports Complex

2. **Community Inclusion Games**
   - Single-day event (June 15, 2025)
   - Features Sitting Volleyball and Boccia Competition
   - Located at Community Recreation Center

3. **Inter-Organization Challenge**
   - Multi-day competition (July 20-22, 2025)
   - Features Swimming Relay
   - Currently inactive (for testing inactive tournaments)

### Sample Activities
- **Tournament Activities**: 5 activities assigned to tournaments
- **Standalone Activities**: 1 activity (Table Tennis) without tournament assignment
- **Ground Allocations**: Each activity has scheduled ground assignments
- **Activity Categories**: Proper ability type associations

### Testing Scenarios
The seed data provides various scenarios for testing:
- Active vs inactive tournaments
- Activities with and without tournament assignments
- Multiple activities per tournament
- Different ground types (arena, pool, courts)
- Various time schedules and durations

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

### Testing with Seed Data
With the seed data, you can immediately test:

```bash
# Get all tournaments
GET /api/tournaments

# Get Spring Championship with all activities
GET /api/tournaments/1

# Get activities for Community Games
GET /api/tournaments/2/activities

# Add Table Tennis to Spring Championship
POST /api/tournaments/1/activities/6
```

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
7. **Comprehensive Testing**: Rich seed data for immediate testing and development