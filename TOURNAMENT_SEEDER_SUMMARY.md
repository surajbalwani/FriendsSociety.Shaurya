# Tournament Demo Data Added to ModelSeeder

## Overview
As requested for Issue #11, demo data for the Tournament entity has been successfully added to the `ModelSeeder.cs` file. This provides comprehensive sample data that demonstrates the Tournament-Activity relationship and the full functionality of the system.

## What Was Added

### 1. Tournament Demo Data
Three diverse tournaments were created to showcase different scenarios:

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

### 2. Enhanced Activity Data
The activities were expanded from 2 to 11 total activities:

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

### 3. Enhanced Ground Infrastructure
Expanded from 2 to 6 grounds to support diverse activities:
- Main Arena (Central Sports Complex)
- Open Ground (Community Park)
- Swimming Pool (Aquatic Center)
- Indoor Court A (Indoor Arena Complex)
- Indoor Court B (Indoor Arena Complex)
- Track Field (Athletics Stadium)

### 4. Comprehensive Ground Allocations
Created 12 ground allocations that span across:
- All tournament activities with realistic scheduling
- Standalone activities for community engagement
- Different time slots and venues appropriate for each sport

### 5. Enhanced Activity Categories
Added proper ability type associations:
- Visual Impairment: Blind Running, Goalball, Blind Football
- Mobility Impairment: Wheelchair Basketball
- Maintained existing associations

## Key Features Demonstrated

### Tournament Hierarchies
- ✅ Tournaments with multiple activities
- ✅ Activities that belong to tournaments
- ✅ Activities that are independent (no tournament)
- ✅ Mixed tournament statuses (active/inactive)

### Realistic Scheduling
- ✅ Tournament date ranges that make sense
- ✅ Activity scheduling within tournament periods
- ✅ Non-overlapping ground allocations
- ✅ Appropriate venues for each sport type

### Data Relationships
- ✅ Tournament → Activities (One-to-Many)
- ✅ Activities → Ground Allocations (One-to-Many)
- ✅ Activities → Activity Categories (Many-to-Many)
- ✅ Ground Allocations → Grounds (Many-to-One)

### Diverse Sports Coverage
- ✅ Wheelchair sports (Basketball, Rugby, Handball)
- ✅ Vision-impaired sports (Blind Running, Goalball, Blind Football)
- ✅ Seated/Adaptive sports (Volleyball, Swimming, Table Tennis)
- ✅ Community wellness (Yoga, Therapeutic Swimming)

## Testing the Implementation

### API Endpoints to Test
```bash
# Get all tournaments
GET /api/tournaments

# Get specific tournament with activities
GET /api/tournaments/1

# Get activities for a tournament
GET /api/tournaments/1/activities

# Get all activities (should show tournament relationships)
GET /api/activities
```

### Expected Behavior
1. **Seeding**: When `SeedDemoData` is enabled, 3 tournaments and 11 activities should be created
2. **Relationships**: Activities should properly reference their parent tournaments
3. **Scheduling**: Ground allocations should be realistic and non-conflicting
4. **API**: Tournament endpoints should return activities with proper nesting

## Configuration
The seeding is controlled by the `DatabaseSettings:SeedDemoData` configuration value in `appsettings.json`.

## Benefits for Development & Testing
- **Rich Dataset**: Provides comprehensive data for testing all Tournament API endpoints
- **Real-world Scenarios**: Covers different tournament types, statuses, and scheduling patterns
- **Relationship Testing**: Demonstrates complex entity relationships and navigation properties
- **UI Development**: Provides meaningful data for frontend development and testing
- **Performance Testing**: Sufficient data volume for testing queries and performance

The implementation successfully addresses the missing demo data for the Tournament entity while maintaining consistency with the existing seeder structure and providing a robust foundation for development and testing.