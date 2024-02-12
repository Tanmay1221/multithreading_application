#!/bin/bash

# Read JSON from input file
input_file="tenant.json"
json_data=$(cat "$input_file")

# Update placeholders in the JSON data
#count=1  # You can replace this with your actual count logic
for count in {16..100}
do
    updated_json=$(echo "$json_data" | jq --arg count "$count" '.baseurl |= gsub("#"; $count) | .tags.product |= gsub("#"; $count) | .relativeurl[] |= (.name |= gsub("#"; $count) | .url |= gsub("#"; $count))')

    # Save the updated JSON to a new file
    output_filename="tenent${count}.json"
    echo "$updated_json" > "$output_filename"

    echo "Updated JSON saved to $output_filename"
done