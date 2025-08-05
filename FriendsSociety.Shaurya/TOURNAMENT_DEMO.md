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

## Demo Data & Seeding

### Overview
Comprehensive demo data has been added to the `ModelSeeder.cs` file to provide realistic sample data for development and testing. The seeding is controlled by the `DatabaseSettings:SeedDemoData` configuration value.

### Sample Tournaments
Three diverse tournaments demonstrate different scenarios:

#### Summer Paralympic Games 2025
- **Duration**: July 15-25, 2025
- **Location**: Central Sports Complex
- **Status**: Active
- **Description**: Annual summer tournament featuring adaptive sports for athletes with various abilities
- **Activities**: 4 activities (Wheelchair Basketball, Blind Running, Wheelchair Rugby, Goalball)

#### Unity Sports Festival
- **Duration**: September 10-12, 2025
- **Location**: Community Sports Hub
- **Status**: Active
- **Description**: A celebration of inclusive sports bringing together athletes of all abilities
- **Activities**: 3 activities (Adaptive Swimming, Seated Volleyball, Para Table Tennis)

#### Winter Challenge Cup
- **Duration**: December 5-8, 2025
- **Location**: Indoor Arena Complex
- **Status**: Inactive (not yet started)
- **Description**: Indoor winter sports competition focusing on team-based activities
- **Activities**: 2 activities (Wheelchair Handball, Blind Football)

### Sample Activities
The seeder creates 11 activities total:

**Tournament-Associated Activities (9):**
- Wheelchair Basketball → Summer Paralympic Games
- Blind Running → Summer Paralympic Games  
- Wheelchair Rugby → Summer Paralympic Games
- Goalball → Summer Paralympic Games
- Adaptive Swimming → Unity Sports Festival
- Seated Volleyball → Unity Sports Festival
- Para Table Tennis → Unity Sports Festival
- Wheelchair Handball → Winter Challenge Cup
- Blind Football → Winter Challenge Cup

**Standalone Activities (2):**
- Community Yoga (no tournament)
- Therapeutic Swimming (no tournament)

### Enhanced Infrastructure
The demo data includes 6 grounds and 12 ground allocations:
- Main Arena (Central Sports Complex)
- Open Ground (Community Park)
- Swimming Pool (Aquatic Center)
- Indoor Court A & B (Indoor Arena Complex)
- Track Field (Athletics Stadium)

### Testing the Demo Data
When seeding is enabled, you can test with:
```bash
# Get all tournaments with their activities
GET /api/tournaments

# Get specific tournament details
GET /api/tournaments/1

# Get activities for a tournament
GET /api/tournaments/1/activities
```

## Benefits

1. **Hierarchical Organization**: Activities can now be grouped under tournaments
2. **Flexible Design**: Activities can exist independently or as part of a tournament
3. **Data Integrity**: Proper foreign key relationships with appropriate delete behavior
4. **Complete CRUD**: Full API operations for tournament management
5. **Soft Deletes**: Non-destructive deletion preserves data integrity
6. **Audit Trail**: Created and updated timestamps for tracking
7. **Rich Demo Data**: Comprehensive sample data for development and testing