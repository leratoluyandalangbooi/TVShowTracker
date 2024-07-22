# TVShowTracker API

TVShowTracker API is a service that allows users to track TV shows, seasons, and episodes. It integrates with The Movie Database (TMDb) to fetch show information and provides user authentication and watchlist functionality.

## Features

- Fetch top TV shows
- Get detailed information about TV shows, seasons, and episodes
- Search for TV shows
- User registration and authentication
- User profile management
- Watchlist functionality to track watched episodes

## API Endpoints

### TV Shows

- `GET /api/shows/top`: Get top TV shows
- `GET /api/shows/{id}`: Get details of a specific TV show
- `PUT /api/shows/{id}`: Update a TV show
- `GET /api/shows/{showId}/season/{seasonNumber}`: Get season details
- `GET /api/shows/{showId}/season/{seasonNumber}/episode/{episodeNumber}`: Get episode details
- `GET /api/shows/search`: Search for TV shows
- `DELETE /api/shows/{cacheKey}`: Clear cache
- `POST /api/shows`: Add a new TV show

### Users

- `POST /api/users/register`: Register a new user
- `POST /api/users/login`: User login
- `GET /api/users/profile`: Get user profile
- `PUT /api/users/profile`: Update user profile
- `PUT /api/users/change-password`: Change user password
- `DELETE /api/users`: Delete user account

### Watched Episodes

- `GET /api/watchlist`: Get user's watched episodes
- `POST /api/watchlist/{episodeId}`: Mark an episode as watched
- `DELETE /api/watchlist/{episodeId}`: Remove an episode from watchlist
- `GET /api/watchlist/{episodeId}/in-watchlist`: Check if an episode is in the watchlist

## Data Models

- TVShows
- Seasons
- Episodes
- Users
- WatchedEpisodes

## Technical Details

- The API is built using ASP.NET Core
- It uses AutoMapper for object mapping
- The TMDb API is used as an external service to fetch TV show data
- Caching is implemented to improve performance
- JWT authentication is used for user authorization

## Getting Started

1. Clone the repository
2. Set up your TMDb API key in the configuration
3. Configure the database connection string
4. Run database migrations
5. Build and run the project

## Configuration

Ensure you have the following settings in your `appsettings.json`:

```json
{
  "TMDbOptions": {
    "ApiKey": "your_tmdb_api_key",
    "BaseUrl": "https://api.themoviedb.org/3/",
    "ReadAccessToken": "your_read_access_token"
  }
}
