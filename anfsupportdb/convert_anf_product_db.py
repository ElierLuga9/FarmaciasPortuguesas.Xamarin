#!/usr/bin/env python
import io
import json
import unicodedata

json_file = open('database.db').read()
json_data = json.loads(json_file)

# The product families.
f = [{"name": "Sa\u00fada Cuida", "color": "532d8e", "fid": 101}, {"name": "Sa\u00fada Fam\u00edlia", "color": "00bbd9", "fid": 102}, {"name": "Sa\u00fada Animal", "color": "fdba12", "fid": 103}]

# Convert the product list.
l = []

# Categories. The fid (family id) must me associated manually.
tmp = {}

for elem in json_data:
	sliced = elem
	sliced.pop(7)
	encoded = [x.encode('utf-8') for x in sliced]
	# Save the category and the matching id.
	tmp[encoded[6]] = encoded[7]
	l.append(dict(zip(["cnp", "pts", "brd", "dsc", "txt", "sup", "cat", "cah"], encoded)))

# Update the CIDs
c = []
for key in tmp:
	val = tmp[key];

	if val == "784a72":
		fid = 101
	elif val == "d6816b":
		fid = 101
	elif val == "344cee":
		fid = 101
	elif val == "609f4c":
		fid = 103
	else:
		fid = 102

	c.append({"name": key, "cid": val, "fid":fid})

d = {"families":f, "categories":c, "products":l}

with open('result.json', 'w') as fp:
	json.dump(d, fp, indent=4, sort_keys=True)

