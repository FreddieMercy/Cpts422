# -*- coding: utf-8 -*-
from __future__ import unicode_literals

from django.db import models, migrations


class Migration(migrations.Migration):

    dependencies = [
        ('api', '0016_auto_20150504_1356'),
    ]

    operations = [
        migrations.AlterField(
            model_name='api',
            name='api_json',
            field=models.TextField(),
            preserve_default=True,
        ),
    ]
