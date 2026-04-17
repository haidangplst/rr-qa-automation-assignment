## TMDB Discover - Test Automation Specifications

### Project Overview
This document outlines comprehensive test scenarios for the TMDB Discover platform (https://tmdb-discover.surge.sh/). The test suite covers filtering options, pagination, and negative test cases for known issues.

---

## Test Suite Overview

### Test Categories
1. **Filtering Tests** - Validate all filter options
2. **Pagination Tests** - Verify pagination functionality
3. **Negative Tests** - Handle known issues gracefully
4. **API Tests** - Validate backend API calls
5. **UI Validation Tests** - Verify UI elements and responses

---

## Detailed Test Scenarios

### 1. CATEGORY FILTERING TESTS

#### TC-001: Filter by Popular Category
**Description:** Verify that users can filter movies/TV shows by "Popular" category
**Steps:**
1. Navigate to https://tmdb-discover.surge.sh/
2. Wait for page to load (verify loading indicator disappears)
3. Click on "Popular" category button
4. Verify API call is made to fetch popular content
5. Verify results are displayed
6. Verify filter button shows "Popular" as active
7. Verify each item shows rating and description

**Expected Result:** 
- Popular content is displayed
- Page shows relevant popular movies/TV shows
- API response contains popular=true or equivalent

**Data Assertions:**
- Response status: 200
- Data array is not empty
- Each item has: id, title, poster_path, vote_average, overview

---

#### TC-002: Filter by Trending Category
**Description:** Verify that users can filter movies/TV shows by "Trending" category
**Steps:**
1. Navigate to https://tmdb-discover.surge.sh/
2. Click on "Trending" category button
3. Verify API call is made to fetch trending content
4. Verify results are displayed
5. Verify filter button shows "Trending" as active
6. Verify items are sorted by trend

**Expected Result:**
- Trending content is displayed in correct order
- API response contains trending content

**Data Assertions:**
- Response includes trending movies/shows
- Items have relevance/trend indicators

---

#### TC-003: Filter by Newest Category
**Description:** Verify filtering by newest released movies/shows
**Steps:**
1. Navigate to home page
2. Click on "Newest" category
3. Verify API call is made
4. Verify results are displayed
5. Verify items are sorted by release date (newest first)

**Expected Result:**
- Newest content is displayed in chronological order
- Release dates are in descending order

---

#### TC-004: Filter by Top Rated Category
**Description:** Verify filtering by top rated movies/shows
**Steps:**
1. Navigate to home page
2. Click on "Top Rated" category
3. Verify API call is made
4. Verify results are displayed
5. Verify items are sorted by rating (highest first)

**Expected Result:**
- Top rated content is displayed
- Items sorted by rating (descending)
- Each item shows its rating

---

### 2. TITLE FILTERING TESTS

#### TC-005: Search by Title - Exact Match
**Description:** Verify users can search for a specific movie/show by title
**Steps:**
1. Navigate to home page
2. Locate search/title input field
3. Enter a known movie title (e.g., "Inception")
4. Press Enter or click Search button
5. Verify API call is made with search query
6. Verify results contain matching title

**Expected Result:**
- Search results contain the queried title
- Matching items are displayed at top

**Data Assertions:**
- API query parameter contains search term
- Response filtered by title

---

#### TC-006: Search by Title - Partial Match
**Description:** Verify partial title matches are supported
**Steps:**
1. Navigate to home page
2. Enter partial title (e.g., "Incep" for "Inception")
3. Verify results show suggestions/matches
4. Verify API handles partial searches

**Expected Result:**
- Partial matches are returned
- Suggestions appear as user types

---

#### TC-007: Search by Title - No Results
**Description:** Verify behavior when search returns no results
**Steps:**
1. Navigate to home page
2. Search for non-existent title (e.g., "XYZ123NoMovie")
3. Verify API call is made
4. Verify no results message is displayed
5. Verify user can clear search and return to main view

**Expected Result:**
- "No results found" message displayed
- User can clear search easily

**Data Assertions:**
- API response status: 200
- Results array is empty

---

### 3. TYPE FILTERING TESTS

#### TC-008: Filter by Movies Type
**Description:** Verify users can filter content to show only movies
**Steps:**
1. Navigate to home page
2. Click on "Movies" type filter
3. Verify API call filters by type=movie
4. Verify only movies are displayed
5. Verify toggle shows "Movies" is active

**Expected Result:**
- Only movies displayed
- No TV shows in results

**Data Assertions:**
- API request includes media_type=movie
- All items have media_type = "movie"

---

#### TC-009: Filter by TV Shows Type
**Description:** Verify users can filter content to show only TV shows
**Steps:**
1. Navigate to home page
2. Click on "TV Shows" type filter
3. Verify API call filters by type=tv
4. Verify only TV shows are displayed
5. Verify toggle shows "TV Shows" is active

**Expected Result:**
- Only TV shows displayed
- No movies in results

**Data Assertions:**
- API request includes media_type=tv
- All items have media_type = "tv"

---

#### TC-010: Toggle Between Movies and TV Shows
**Description:** Verify user can switch between movies and TV shows
**Steps:**
1. Filter by Movies
2. Verify only movies shown
3. Switch to TV Shows
4. Verify only TV shows shown
5. Switch back to Movies
6. Verify movies shown again

**Expected Result:**
- Content updates correctly when toggling
- No mixed content types

---

### 4. YEAR OF RELEASE FILTERING TESTS

#### TC-011: Filter by Recent Year (Current Year)
**Description:** Verify filtering by current year shows only current year releases
**Steps:**
1. Navigate to home page
2. Click on Year filter
3. Select current year (2024/2025)
4. Verify API call includes year parameter
5. Verify results show items from selected year

**Expected Result:**
- Results contain only items from selected year
- Release dates match the selected year

**Data Assertions:**
- API request includes year/release_date parameter
- All items have release_year matching selected year

---

#### TC-012: Filter by Multiple Years
**Description:** Verify user can filter by multiple year ranges
**Steps:**
1. Navigate to home page
2. Click on Year filter
3. Select years: 2020-2024
4. Verify API call reflects year range
5. Verify results span selected years

**Expected Result:**
- Results include items from all selected years
- No items outside the range

---

#### TC-013: Filter by Specific Past Year
**Description:** Verify filtering by specific past year
**Steps:**
1. Navigate to home page
2. Filter by year 2000
3. Verify results show items from year 2000
4. Verify results can be paginated

**Expected Result:**
- Items from specified year displayed
- Pagination works for year results

---

### 5. RATING FILTERING TESTS

#### TC-014: Filter by High Rating (8+)
**Description:** Verify filtering by high-rated content (rating >= 8)
**Steps:**
1. Navigate to home page
2. Click on Rating filter
3. Select rating 8+
4. Verify API call includes rating parameter
5. Verify results show items with rating >= 8

**Expected Result:**
- All results have rating >= 8
- Items sorted by rating (descending)

**Data Assertions:**
- API request includes vote_average_gte=8
- All items have vote_average >= 8.0

---

#### TC-015: Filter by Medium Rating (5-7)
**Description:** Verify filtering by medium-rated content
**Steps:**
1. Navigate to home page
2. Click on Rating filter
3. Select rating 5-7 range
4. Verify results show items with rating 5-7

**Expected Result:**
- All results have 5 <= rating <= 7
- Appropriate items displayed

---

#### TC-016: Filter by Low Rating (< 5)
**Description:** Verify filtering by low-rated content
**Steps:**
1. Navigate to home page
2. Click on Rating filter
3. Select rating < 5
4. Verify results show items with rating < 5

**Expected Result:**
- Results show low-rated items
- Can still view details of low-rated content

---

### 6. GENRE FILTERING TESTS

#### TC-017: Filter by Single Genre (Action)
**Description:** Verify filtering by single genre
**Steps:**
1. Navigate to home page
2. Click on Genre filter
3. Select "Action" genre
4. Verify API call includes genre parameter
5. Verify results contain action content

**Expected Result:**
- Results contain action movies/shows
- Genre tag displayed in results

**Data Assertions:**
- API request includes genre_id parameter
- Items have "Action" in genres array

---

#### TC-018: Filter by Multiple Genres
**Description:** Verify filtering by multiple genres simultaneously
**Steps:**
1. Navigate to home page
2. Click on Genre filter
3. Select "Action" and "Drama"
4. Verify API includes both genres
5. Verify results contain items with both genres

**Expected Result:**
- Items have one or both selected genres
- Multiple genre selection works correctly

---

#### TC-019: Filter by All Available Genres
**Description:** Verify all genre options are available and functional
**Steps:**
1. Navigate to home page
2. Click on Genre filter
3. Verify all genres are listed
4. Click through each genre
5. Verify results update for each

**Expected Result:**
- All genres selectable and functional
- Results vary appropriately for each genre

---

#### TC-020: Clear Genre Filter
**Description:** Verify user can clear genre filter
**Steps:**
1. Select a genre
2. Verify results filtered
3. Click "Clear" or deselect genre
4. Verify all genres shown again

**Expected Result:**
- Filter cleared successfully
- All content displayed again

---

### 7. COMBINED FILTERING TESTS

#### TC-021: Combine Category + Type Filter
**Description:** Verify multiple filter combinations work together
**Steps:**
1. Select "Popular" category
2. Select "Movies" type
3. Verify API call includes both filters
4. Verify results are popular movies only

**Expected Result:**
- Filters combine correctly (AND logic)
- Results match all criteria

---

#### TC-022: Combine Type + Year + Rating
**Description:** Verify complex multi-filter scenarios
**Steps:**
1. Filter by: TV Shows + Year 2020-2023 + Rating 7+
2. Verify API call includes all parameters
3. Verify results match all criteria
4. Verify pagination works with filters

**Expected Result:**
- All filters applied simultaneously
- Results accurate to all criteria

---

#### TC-023: Combine All Filters
**Description:** Verify applying multiple filters at once
**Steps:**
1. Apply filters: Popular + Movies + 2020-2024 + Rating 8+ + Genre: Action
2. Verify API request includes all parameters
3. Verify results satisfy all criteria

**Expected Result:**
- All filters work together
- Results are accurate and paginated

---

### 8. PAGINATION TESTS

#### TC-024: Navigate to Next Page
**Description:** Verify pagination - moving to next page
**Steps:**
1. Navigate to home page
2. Verify page 1 is displayed with 20 items
3. Click "Next" or page 2 button
4. Verify API call made with page=2 parameter
5. Verify different results displayed (page 2 items)
6. Verify previous button is enabled

**Expected Result:**
- Next page loaded successfully
- Different items displayed
- Page number updated

**Data Assertions:**
- API request includes page=2
- Results array contains 20 items
- Items are different from page 1

---

#### TC-025: Navigate to Previous Page
**Description:** Verify pagination - moving to previous page
**Steps:**
1. Navigate to page 2
2. Click "Previous" button
3. Verify API call made with page=1 parameter
4. Verify page 1 items displayed again

**Expected Result:**
- Previous page loaded
- Original page 1 items shown

---

#### TC-026: Jump to Specific Page
**Description:** Verify user can jump to specific page number
**Steps:**
1. Click on page number 5 directly
2. Verify API call made with page=5 parameter
3. Verify page 5 results displayed

**Expected Result:**
- Correct page loaded directly
- No intermediate page loads

---

#### TC-027: Pagination with Filters Applied
**Description:** Verify pagination works when filters are active
**Steps:**
1. Apply filter: "Popular" category
2. Navigate through pages
3. Verify filters remain active on each page
4. Verify correct filtered results on each page

**Expected Result:**
- Filters maintained across pages
- Results consistently filtered

---

#### TC-028: Last Page Behavior (NEGATIVE TEST)
**Description:** Verify behavior on last page (known issue)
**Steps:**
1. Navigate to last available page
2. Verify "Next" button is disabled
3. Attempt to load beyond last page
4. Document any errors

**Expected Result:**
- Graceful handling of last page
- No errors when attempting next

**Known Issue:** Pagination may not work properly on last pages

---

#### TC-029: Page Navigation Preserves Scroll Position
**Description:** Verify page maintains user experience
**Steps:**
1. Load page 1
2. Scroll down to 50% of page
3. Click next page
4. Verify page scrolls to top

**Expected Result:**
- Page scrolls to top for new content
- User can immediately see results

---

### 9. NEGATIVE TEST CASES

#### TC-030: Direct URL Access with Slug (NEGATIVE TEST)
**Description:** Verify behavior when accessing page via slug
**Steps:**
1. Navigate to https://tmdb-discover.surge.sh/popular
2. Verify page loads (or handles error gracefully)
3. Document any errors or unexpected behavior

**Expected Result:**
- Page loads correctly OR
- User is redirected to home page with appropriate filter applied

**Known Issue:** Slug-based URLs may not work as expected

---

#### TC-031: Multiple Slug Access (NEGATIVE TEST)
**Description:** Test various slug combinations
**Steps:**
1. Try https://tmdb-discover.surge.sh/movies
2. Try https://tmdb-discover.surge.sh/tv-shows
3. Try https://tmdb-discover.surge.sh/2024
4. Document behavior

**Expected Result:**
- Graceful handling of invalid routes
- Proper error messages or redirects

---

#### TC-032: Invalid Page Number (NEGATIVE TEST)
**Description:** Verify behavior with invalid page parameters
**Steps:**
1. Attempt to access page 0
2. Attempt to access page 99999
3. Attempt to access page -1
4. Verify error handling

**Expected Result:**
- Default to valid page (1 or 20)
- Error message displayed
- No application crash

---

#### TC-033: Missing API Response (NEGATIVE TEST)
**Description:** Verify app handles missing/delayed API responses
**Steps:**
1. Open browser DevTools Network tab
2. Throttle connection to "Slow 3G"
3. Apply a filter
4. Verify loading state shown
5. Verify timeout or data eventually loads

**Expected Result:**
- Loading indicator shown
- User informed of delay
- Data loads or timeout handled gracefully

---

#### TC-034: Browser Back Button Behavior
**Description:** Verify back button works correctly with filters
**Steps:**
1. Apply filter: "Popular"
2. View a movie detail (if available)
3. Press browser back button
4. Verify previous filter state restored

**Expected Result:**
- Filter state preserved in browser history
- Back button works as expected

---

### 10. UI VALIDATION TESTS

#### TC-035: Page Load Completeness
**Description:** Verify all page elements load correctly
**Steps:**
1. Navigate to home page
2. Verify header loaded
3. Verify filter panel loaded
4. Verify results grid loaded
5. Verify pagination controls loaded

**Expected Result:**
- All elements visible
- No missing UI components

---

#### TC-036: Responsive Design - Desktop
**Description:** Verify layout on desktop (1920x1080)
**Steps:**
1. Set viewport to 1920x1080
2. Navigate to page
3. Verify all elements properly aligned
4. Verify grid layout correct

**Expected Result:**
- Layout looks correct
- No overflow or missing elements

---

#### TC-037: Responsive Design - Tablet
**Description:** Verify layout on tablet (768x1024)
**Steps:**
1. Set viewport to 768x1024
2. Navigate to page
3. Verify elements responsive
4. Verify readable on tablet size

**Expected Result:**
- Layout adapts to tablet
- Content readable

---

#### TC-038: Responsive Design - Mobile
**Description:** Verify layout on mobile (375x667)
**Steps:**
1. Set viewport to 375x667
2. Navigate to page
3. Verify mobile menu or filters accessible
4. Verify grid stacks properly

**Expected Result:**
- Mobile layout functional
- Touch-friendly interface

---

#### TC-039: Accessibility - Keyboard Navigation
**Description:** Verify keyboard navigation works
**Steps:**
1. Navigate with Tab key only
2. Verify all interactive elements focusable
3. Verify focus visible on filters and buttons
4. Verify Enter key activates buttons

**Expected Result:**
- Full keyboard navigation possible
- Focus indicators visible

---

#### TC-040: Accessibility - Color Contrast
**Description:** Verify proper color contrast
**Steps:**
1. Check text color contrast ratios
2. Verify readable for colorblind users (if applicable)
3. Verify button contrast sufficient

**Expected Result:**
- WCAG AA compliance
- Text readable

---

### 11. PERFORMANCE TESTS

#### TC-041: Initial Page Load Time
**Description:** Verify page loads within reasonable time
**Steps:**
1. Navigate to home page
2. Measure time to first paint
3. Measure time to interactive
4. Document metrics

**Expected Result:**
- First paint < 2 seconds
- Time to interactive < 5 seconds

---

#### TC-042: Filter Application Speed
**Description:** Verify filters apply quickly
**Steps:**
1. Apply filter
2. Measure time until results update
3. Document API call time

**Expected Result:**
- Filter applied < 1 second
- API response < 500ms

---

#### TC-043: Large Result Set Handling
**Description:** Verify app handles large datasets
**Steps:**
1. Apply filter with many results
2. Verify pagination works
3. Verify no performance degradation

**Expected Result:**
- App remains responsive
- Pagination works smoothly

---

## Test Data Requirements

### Sample Movie Titles to Search
- "Inception"
- "The Dark Knight"
- "Avatar"
- "Interstellar"
- "Avengers"

### Genre Options (Expected)
- Action
- Drama
- Comedy
- Horror
- Sci-Fi
- Animation
- Romance
- Thriller

### Year Range
- 1990-2025 (or relevant range)

### Rating Scale
- 0-10 (IMDb style)

---

## API Contract Expectations

### GET /discover/movie
**Expected Parameters:**
- page (integer)
- sort_by (string): popularity, rating, release_date
- with_genres (integer array)
- vote_average.gte (number)
- primary_release_year (integer)
- language (string)
- media_type (string)

**Expected Response:**
```json
{
  "results": [
    {
      "id": 123,
      "title": "Movie Title",
      "overview": "Description",
      "poster_path": "/path/to/poster.jpg",
      "backdrop_path": "/path/to/backdrop.jpg",
      "vote_average": 7.5,
      "release_date": "2024-01-01",
      "genre_ids": [28, 12, 16]
    }
  ],
  "page": 1,
  "total_pages": 50,
  "total_results": 1000
}
```

---

## Test Execution Strategy

### Test Environment
- Browser: Chrome (Chromium)
- Environment: https://tmdb-discover.surge.sh/
- Resolution: 1920x1080 (default)

### Test Execution Order
1. UI Validation Tests (TC-035 to TC-040)
2. Category Filtering Tests (TC-001 to TC-004)
3. Type Filtering Tests (TC-008 to TC-010)
4. Title Search Tests (TC-005 to TC-007)
5. Year Filtering Tests (TC-011 to TC-013)
6. Rating Filtering Tests (TC-014 to TC-016)
7. Genre Filtering Tests (TC-017 to TC-020)
8. Combined Filtering Tests (TC-021 to TC-023)
9. Pagination Tests (TC-024 to TC-029)
10. Negative Tests (TC-030 to TC-034)
11. Performance Tests (TC-041 to TC-043)

### Test Execution Groups
- **Smoke Tests**: TC-001, TC-008, TC-024
- **Critical Path**: TC-001 to TC-023, TC-024, TC-035
- **Extended Tests**: All tests
- **Negative Tests**: TC-030 to TC-034

---

## Reporting Requirements

### Test Report Contents
1. Summary statistics (total, passed, failed, skipped)
2. Defects found with severity
3. Screenshots for failed tests
4. Console logs for debugging
5. API response payloads
6. Test execution duration

### Defect Report Template
- Defect ID
- Test Case ID
- Title
- Steps to reproduce
- Expected vs. Actual
- Environment details
- Screenshots/videos
- Severity (Critical, High, Medium, Low)

---

## Known Issues to Document

1. **Direct URL Access**: Accessing via slug URLs (e.g., /popular) may not work
2. **Last Page Pagination**: Last few pages may not paginate properly
3. **Search Performance**: Search may be slow with large result sets

---

## Success Criteria

✅ All critical path tests (Smoke + Core functionality) pass  
✅ All filters work independently and in combination  
✅ Pagination works for first ~20 pages  
✅ API calls return correct data  
✅ UI responsive and accessible  
✅ Performance acceptable (< 5s load time)  
✅ Negative tests handled gracefully  
✅ Logging captures all relevant events  
✅ HTML report generated with results  
