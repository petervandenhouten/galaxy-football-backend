# This file is auto-generated. Do not edit manually.
class_name GalaxyFootballDtos
extends RefCounted

class LeagueStandingsResponseDto:
    var api_version: String = ""
    var generated_at: String = ""
    var league: LeagueReferenceDto = null
    var season: SeasonInfoDto = null
    var standings: Array[LeagueStandingEntryDto] = []

static func parse_league_standings_response_dto(data: Variant) -> LeagueStandingsResponseDto:
    if data == null:
        return null
    if not (data is Dictionary):
        push_warning("Expected Dictionary for LeagueStandingsResponseDto, got %s." % type_string(typeof(data)))
        return null

    var source: Dictionary = data
    var dto := LeagueStandingsResponseDto.new()
    if not source.has("apiVersion") or source.get("apiVersion") == null:
        push_warning("Missing required field 'apiVersion' while parsing LeagueStandingsResponseDto.")
    dto.api_version = dto_to_string(source.get("apiVersion"))
    if not source.has("generatedAt") or source.get("generatedAt") == null:
        push_warning("Missing required field 'generatedAt' while parsing LeagueStandingsResponseDto.")
    dto.generated_at = dto_to_string(source.get("generatedAt"))
    if not source.has("league") or source.get("league") == null:
        push_warning("Missing required field 'league' while parsing LeagueStandingsResponseDto.")
    dto.league = parse_league_reference_dto(source.get("league"))
    if not source.has("season") or source.get("season") == null:
        push_warning("Missing required field 'season' while parsing LeagueStandingsResponseDto.")
    dto.season = parse_season_info_dto(source.get("season"))
    if not source.has("standings") or source.get("standings") == null:
        push_warning("Missing required field 'standings' while parsing LeagueStandingsResponseDto.")
    dto.standings = []
    var standings_values = source.get("standings")
    if standings_values is Array:
        for item in standings_values:
            var parsed_item := parse_league_standing_entry_dto(item)
            if parsed_item != null:
                dto.standings.append(parsed_item)
            else:
                push_warning("Skipping invalid item in 'standings' while parsing LeagueStandingsResponseDto.")
    elif standings_values != null:
        push_warning("Expected Array for 'standings', got %s." % type_string(typeof(standings_values)))
    return dto

static func parse_json_league_standings_response_dto(json_text: String) -> LeagueStandingsResponseDto:
    var json := JSON.new()
    var parse_result = json.parse(json_text)
    if parse_result != OK:
        push_error("Failed to parse JSON for LeagueStandingsResponseDto: %s at line %d." % [json.get_error_message(), json.get_error_line()])
        return null
    return parse_league_standings_response_dto(json.data)

class LeagueReferenceDto:
    var id: String = ""
    var level: int = 0
    var number: int = 0

static func parse_league_reference_dto(data: Variant) -> LeagueReferenceDto:
    if data == null:
        return null
    if not (data is Dictionary):
        push_warning("Expected Dictionary for LeagueReferenceDto, got %s." % type_string(typeof(data)))
        return null

    var source: Dictionary = data
    var dto := LeagueReferenceDto.new()
    if not source.has("id") or source.get("id") == null:
        push_warning("Missing required field 'id' while parsing LeagueReferenceDto.")
    dto.id = dto_to_string(source.get("id"))
    if not source.has("level") or source.get("level") == null:
        push_warning("Missing required field 'level' while parsing LeagueReferenceDto.")
    dto.level = dto_to_int(source.get("level"))
    if not source.has("number") or source.get("number") == null:
        push_warning("Missing required field 'number' while parsing LeagueReferenceDto.")
    dto.number = dto_to_int(source.get("number"))
    return dto

static func parse_json_league_reference_dto(json_text: String) -> LeagueReferenceDto:
    var json := JSON.new()
    var parse_result = json.parse(json_text)
    if parse_result != OK:
        push_error("Failed to parse JSON for LeagueReferenceDto: %s at line %d." % [json.get_error_message(), json.get_error_line()])
        return null
    return parse_league_reference_dto(json.data)

class SeasonInfoDto:
    var year: Variant = null
    var day: Variant = null

static func parse_season_info_dto(data: Variant) -> SeasonInfoDto:
    if data == null:
        return null
    if not (data is Dictionary):
        push_warning("Expected Dictionary for SeasonInfoDto, got %s." % type_string(typeof(data)))
        return null

    var source: Dictionary = data
    var dto := SeasonInfoDto.new()
    dto.year = dto_to_nullable_int(source.get("year"))
    dto.day = dto_to_nullable_int(source.get("day"))
    return dto

static func parse_json_season_info_dto(json_text: String) -> SeasonInfoDto:
    var json := JSON.new()
    var parse_result = json.parse(json_text)
    if parse_result != OK:
        push_error("Failed to parse JSON for SeasonInfoDto: %s at line %d." % [json.get_error_message(), json.get_error_line()])
        return null
    return parse_season_info_dto(json.data)

class LeagueStandingEntryDto:
    var rank: int = 0
    var previous_rank: int = 0
    var team_id: String = ""
    var team_name: String = ""
    var points: int = 0
    var played: int = 0
    var wins: int = 0
    var draws: int = 0
    var losses: int = 0
    var goals_for: int = 0
    var goals_against: int = 0
    var goal_difference: int = 0
    var form: Array[String] = []

static func parse_league_standing_entry_dto(data: Variant) -> LeagueStandingEntryDto:
    if data == null:
        return null
    if not (data is Dictionary):
        push_warning("Expected Dictionary for LeagueStandingEntryDto, got %s." % type_string(typeof(data)))
        return null

    var source: Dictionary = data
    var dto := LeagueStandingEntryDto.new()
    if not source.has("rank") or source.get("rank") == null:
        push_warning("Missing required field 'rank' while parsing LeagueStandingEntryDto.")
    dto.rank = dto_to_int(source.get("rank"))
    if not source.has("previousRank") or source.get("previousRank") == null:
        push_warning("Missing required field 'previousRank' while parsing LeagueStandingEntryDto.")
    dto.previous_rank = dto_to_int(source.get("previousRank"))
    if not source.has("teamId") or source.get("teamId") == null:
        push_warning("Missing required field 'teamId' while parsing LeagueStandingEntryDto.")
    dto.team_id = dto_to_string(source.get("teamId"))
    if not source.has("teamName") or source.get("teamName") == null:
        push_warning("Missing required field 'teamName' while parsing LeagueStandingEntryDto.")
    dto.team_name = dto_to_string(source.get("teamName"))
    if not source.has("points") or source.get("points") == null:
        push_warning("Missing required field 'points' while parsing LeagueStandingEntryDto.")
    dto.points = dto_to_int(source.get("points"))
    if not source.has("played") or source.get("played") == null:
        push_warning("Missing required field 'played' while parsing LeagueStandingEntryDto.")
    dto.played = dto_to_int(source.get("played"))
    if not source.has("wins") or source.get("wins") == null:
        push_warning("Missing required field 'wins' while parsing LeagueStandingEntryDto.")
    dto.wins = dto_to_int(source.get("wins"))
    if not source.has("draws") or source.get("draws") == null:
        push_warning("Missing required field 'draws' while parsing LeagueStandingEntryDto.")
    dto.draws = dto_to_int(source.get("draws"))
    if not source.has("losses") or source.get("losses") == null:
        push_warning("Missing required field 'losses' while parsing LeagueStandingEntryDto.")
    dto.losses = dto_to_int(source.get("losses"))
    if not source.has("goalsFor") or source.get("goalsFor") == null:
        push_warning("Missing required field 'goalsFor' while parsing LeagueStandingEntryDto.")
    dto.goals_for = dto_to_int(source.get("goalsFor"))
    if not source.has("goalsAgainst") or source.get("goalsAgainst") == null:
        push_warning("Missing required field 'goalsAgainst' while parsing LeagueStandingEntryDto.")
    dto.goals_against = dto_to_int(source.get("goalsAgainst"))
    if not source.has("goalDifference") or source.get("goalDifference") == null:
        push_warning("Missing required field 'goalDifference' while parsing LeagueStandingEntryDto.")
    dto.goal_difference = dto_to_int(source.get("goalDifference"))
    if not source.has("form") or source.get("form") == null:
        push_warning("Missing required field 'form' while parsing LeagueStandingEntryDto.")
    dto.form = []
    var form_values = source.get("form")
    if form_values is Array:
        for item in form_values:
            dto.form.append(dto_to_string(item))
    elif form_values != null:
        push_warning("Expected Array for 'form', got %s." % type_string(typeof(form_values)))
    return dto

static func parse_json_league_standing_entry_dto(json_text: String) -> LeagueStandingEntryDto:
    var json := JSON.new()
    var parse_result = json.parse(json_text)
    if parse_result != OK:
        push_error("Failed to parse JSON for LeagueStandingEntryDto: %s at line %d." % [json.get_error_message(), json.get_error_line()])
        return null
    return parse_league_standing_entry_dto(json.data)

static func dto_to_string(value: Variant) -> String:
    if value == null:
        return ""
    return str(value)

static func dto_to_nullable_string(value: Variant) -> Variant:
    if value == null:
        return null
    return str(value)

static func dto_to_int(value: Variant) -> int:
    if value == null:
        return 0
    return int(value)

static func dto_to_nullable_int(value: Variant) -> Variant:
    if value == null:
        return null
    return int(value)

static func dto_to_float(value: Variant) -> float:
    if value == null:
        return 0.0
    return float(value)

static func dto_to_nullable_float(value: Variant) -> Variant:
    if value == null:
        return null
    return float(value)

static func dto_to_bool(value: Variant) -> bool:
    if value == null:
        return false
    return bool(value)

static func dto_to_nullable_bool(value: Variant) -> Variant:
    if value == null:
        return null
    return bool(value)
