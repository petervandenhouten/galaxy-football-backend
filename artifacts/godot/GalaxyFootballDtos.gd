# This file is auto-generated. Do not edit manually.
class_name GalaxyFootballDtos
extends RefCounted

class LeagueStandingsResponseDto:
    var api_version = ""
    var generated_at = ""
    var league = null
    var season = null
    var standings = []

static func parse_league_standings_response_dto(data: Variant) -> LeagueStandingsResponseDto:
    if data == null or not (data is Dictionary):
        return null

    var source: Dictionary = data
    var dto := LeagueStandingsResponseDto.new()
    dto.api_version = _to_string(source.get("apiVersion"))
    dto.generated_at = _to_string(source.get("generatedAt"))
    dto.league = parse_league_reference_dto(source.get("league"))
    dto.season = parse_season_info_dto(source.get("season"))
    dto.standings = []
    var standings_values = source.get("standings")
    if standings_values is Array:
        for item in standings_values:
            dto.standings.append(parse_league_standing_entry_dto(item))
    return dto

static func parse_json_league_standings_response_dto(json_text: String) -> LeagueStandingsResponseDto:
    var parsed = JSON.parse_string(json_text)
    return parse_league_standings_response_dto(parsed)

class LeagueReferenceDto:
    var id = ""
    var level = 0
    var number = 0

static func parse_league_reference_dto(data: Variant) -> LeagueReferenceDto:
    if data == null or not (data is Dictionary):
        return null

    var source: Dictionary = data
    var dto := LeagueReferenceDto.new()
    dto.id = _to_string(source.get("id"))
    dto.level = _to_int(source.get("level"))
    dto.number = _to_int(source.get("number"))
    return dto

static func parse_json_league_reference_dto(json_text: String) -> LeagueReferenceDto:
    var parsed = JSON.parse_string(json_text)
    return parse_league_reference_dto(parsed)

class SeasonInfoDto:
    var year = null
    var day = null

static func parse_season_info_dto(data: Variant) -> SeasonInfoDto:
    if data == null or not (data is Dictionary):
        return null

    var source: Dictionary = data
    var dto := SeasonInfoDto.new()
    dto.year = _to_nullable_int(source.get("year"))
    dto.day = _to_nullable_int(source.get("day"))
    return dto

static func parse_json_season_info_dto(json_text: String) -> SeasonInfoDto:
    var parsed = JSON.parse_string(json_text)
    return parse_season_info_dto(parsed)

class LeagueStandingEntryDto:
    var rank = 0
    var previous_rank = 0
    var team_id = ""
    var team_name = ""
    var points = 0
    var played = 0
    var wins = 0
    var draws = 0
    var losses = 0
    var goals_for = 0
    var goals_against = 0
    var goal_difference = 0
    var form = []

static func parse_league_standing_entry_dto(data: Variant) -> LeagueStandingEntryDto:
    if data == null or not (data is Dictionary):
        return null

    var source: Dictionary = data
    var dto := LeagueStandingEntryDto.new()
    dto.rank = _to_int(source.get("rank"))
    dto.previous_rank = _to_int(source.get("previousRank"))
    dto.team_id = _to_string(source.get("teamId"))
    dto.team_name = _to_string(source.get("teamName"))
    dto.points = _to_int(source.get("points"))
    dto.played = _to_int(source.get("played"))
    dto.wins = _to_int(source.get("wins"))
    dto.draws = _to_int(source.get("draws"))
    dto.losses = _to_int(source.get("losses"))
    dto.goals_for = _to_int(source.get("goalsFor"))
    dto.goals_against = _to_int(source.get("goalsAgainst"))
    dto.goal_difference = _to_int(source.get("goalDifference"))
    dto.form = []
    var form_values = source.get("form")
    if form_values is Array:
        for item in form_values:
            dto.form.append(_to_string(item))
    return dto

static func parse_json_league_standing_entry_dto(json_text: String) -> LeagueStandingEntryDto:
    var parsed = JSON.parse_string(json_text)
    return parse_league_standing_entry_dto(parsed)

static func _to_string(value: Variant) -> String:
    if value == null:
        return ""
    return str(value)

static func _to_nullable_string(value: Variant) -> Variant:
    if value == null:
        return null
    return str(value)

static func _to_int(value: Variant) -> int:
    if value == null:
        return 0
    return int(value)

static func _to_nullable_int(value: Variant) -> Variant:
    if value == null:
        return null
    return int(value)

static func _to_float(value: Variant) -> float:
    if value == null:
        return 0.0
    return float(value)

static func _to_nullable_float(value: Variant) -> Variant:
    if value == null:
        return null
    return float(value)

static func _to_bool(value: Variant) -> bool:
    if value == null:
        return false
    return bool(value)

static func _to_nullable_bool(value: Variant) -> Variant:
    if value == null:
        return null
    return bool(value)
